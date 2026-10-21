using System.Text;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public interface ILibraryDepthService
{
    Task<LibraryBookIssue> IssueAsync(Guid copyId, Guid memberId, DateOnly issueDate, CancellationToken token = default);
    Task<LibraryReservation> ReserveAsync(Guid bookId, Guid memberId, CancellationToken token = default);
    Task<LibraryBookReturn> ReturnAsync(Guid issueId, LibraryItemCondition condition, string? notes, CancellationToken token = default);
    Task<LibraryItemIncident> ReportIncidentAsync(Guid copyId, Guid memberId, Guid? issueId, LibraryItemCondition condition, string details, decimal? assessedAmount, CancellationToken token = default);
    Task<FeeInvoice> EnsureFineInvoiceAsync(Guid fineId, CancellationToken token = default);
    Task<int> ProcessQueuesAndOverduesAsync(CancellationToken token = default);
    Task<LibraryStockVerificationItem> VerifyStockAsync(Guid verificationId, string scanCode, CancellationToken token = default);
    string CreateCode39Svg(string value, string caption);
}

public sealed class LibraryDepthService(TenantDbContext db, IFinanceAccountingService finance) : ILibraryDepthService
{
    public async Task<LibraryBookIssue> IssueAsync(Guid copyId, Guid memberId, DateOnly issueDate, CancellationToken token = default)
    {
        var copy = await db.LibraryBookCopies.SingleAsync(x => x.Id == copyId, token);
        var member = await db.LibraryMembers.SingleAsync(x => x.Id == memberId, token);
        if (!member.IsActive || member.ExpiresOn < issueDate) throw new InvalidOperationException("The library membership is not active.");
        if (copy.Status is not BookCopyStatus.Available and not BookCopyStatus.Reserved) throw new InvalidOperationException("This copy is not available for issue.");
        var policy = await Policy(member, token);
        var active = await db.LibraryBookIssues.CountAsync(x => x.LibraryMemberId == memberId && (x.Status == LibraryIssueStatus.Issued || x.Status == LibraryIssueStatus.Overdue), token);
        var limit = Math.Min(member.MaxBooksAllowed, policy.MaxIssues);
        if (active >= limit) throw new InvalidOperationException($"The member has reached the issue limit of {limit} books.");
        var reservation = await db.LibraryReservations.Where(x => x.LibraryBookId == copy.LibraryBookId && (x.Status == LibraryReservationStatus.Requested || x.Status == LibraryReservationStatus.Active)).OrderBy(x => x.QueuePosition).ThenBy(x => x.ReservedOn).FirstOrDefaultAsync(token);
        if (reservation is not null && reservation.LibraryMemberId != memberId) throw new InvalidOperationException("This title is reserved for another member at the head of the queue.");
        var issue = new LibraryBookIssue { TenantId = member.TenantId, BranchId = copy.BranchId, LibraryBookCopyId = copy.Id, LibraryMemberId = member.Id, IssueDate = issueDate, DueDate = issueDate.AddDays(policy.LoanDays), Status = LibraryIssueStatus.Issued };
        copy.Status = BookCopyStatus.Issued;
        if (reservation is not null) { reservation.Status = LibraryReservationStatus.Fulfilled; reservation.FulfilledOn = DateTime.UtcNow; reservation.LibraryBookCopyId = copy.Id; }
        db.LibraryBookIssues.Add(issue);
        await db.SaveChangesAsync(token);
        return issue;
    }

    public async Task<LibraryReservation> ReserveAsync(Guid bookId, Guid memberId, CancellationToken token = default)
    {
        var member = await db.LibraryMembers.SingleAsync(x => x.Id == memberId, token);
        if (await db.LibraryReservations.AnyAsync(x => x.LibraryBookId == bookId && x.LibraryMemberId == memberId && (x.Status == LibraryReservationStatus.Requested || x.Status == LibraryReservationStatus.Active), token)) throw new InvalidOperationException("An active reservation already exists for this title.");
        var policy = await Policy(member, token);
        var position = await db.LibraryReservations.CountAsync(x => x.LibraryBookId == bookId && (x.Status == LibraryReservationStatus.Requested || x.Status == LibraryReservationStatus.Active), token) + 1;
        var available = await db.LibraryBookCopies.Where(x => x.LibraryBookId == bookId && x.Status == BookCopyStatus.Available).OrderBy(x => x.AccessionNumber).FirstOrDefaultAsync(token);
        var reservation = new LibraryReservation { TenantId = member.TenantId, BranchId = member.BranchId, LibraryBookId = bookId, LibraryMemberId = memberId, LibraryBookCopyId = available?.Id, QueuePosition = position, Status = available is null ? LibraryReservationStatus.Requested : LibraryReservationStatus.Active, ExpiresOn = DateTime.UtcNow.AddDays(policy.ReservationHoldDays) };
        if (available is not null) { available.Status = BookCopyStatus.Reserved; reservation.AvailabilityNotifiedOn = DateTime.UtcNow; await QueueAvailabilityNotice(member, bookId, reservation.ExpiresOn, token); }
        db.LibraryReservations.Add(reservation);
        await db.SaveChangesAsync(token);
        return reservation;
    }

    public async Task<LibraryBookReturn> ReturnAsync(Guid issueId, LibraryItemCondition condition, string? notes, CancellationToken token = default)
    {
        var issue = await db.LibraryBookIssues.Include(x => x.LibraryMember).Include(x => x.LibraryBookCopy).SingleAsync(x => x.Id == issueId, token);
        if (issue.Status == LibraryIssueStatus.Returned) throw new InvalidOperationException("This issue is already returned.");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var policy = await Policy(issue.LibraryMember!, token);
        var overdueDays = Math.Max(0, today.DayNumber - issue.DueDate.DayNumber - policy.GraceDays);
        var fineAmount = Math.Min(policy.MaximumFine, overdueDays * policy.FinePerDay);
        var returned = new LibraryBookReturn { TenantId = issue.TenantId, BranchId = issue.BranchId, LibraryBookIssueId = issue.Id, ReturnedOn = DateTime.UtcNow, FineAssessed = fineAmount, ConditionNotes = notes };
        issue.ReturnedOn = today; issue.Status = LibraryIssueStatus.Returned; issue.FineAmount = fineAmount;
        issue.LibraryBookCopy!.Status = condition == LibraryItemCondition.Damaged ? BookCopyStatus.Damaged : BookCopyStatus.Available;
        db.LibraryBookReturns.Add(returned);
        if (fineAmount > 0)
        {
            var fine = new LibraryFineRecord { TenantId = issue.TenantId, BranchId = issue.BranchId, LibraryMemberId = issue.LibraryMemberId, LibraryBookIssueId = issue.Id, Amount = fineAmount, Reason = $"Overdue by {overdueDays} day(s)", Status = LibraryFineStatus.Pending };
            db.LibraryFineRecords.Add(fine);
            await db.SaveChangesAsync(token);
            await EnsureFineInvoiceAsync(fine.Id, token);
        }
        else await db.SaveChangesAsync(token);
        await ActivateNextReservation(issue.LibraryBookCopy.LibraryBookId, issue.LibraryBookCopy, token);
        await db.SaveChangesAsync(token);
        return returned;
    }

    public async Task<LibraryItemIncident> ReportIncidentAsync(Guid copyId, Guid memberId, Guid? issueId, LibraryItemCondition condition, string details, decimal? assessedAmount, CancellationToken token = default)
    {
        if (condition is not LibraryItemCondition.Lost and not LibraryItemCondition.Damaged) throw new InvalidOperationException("Only lost or damaged incidents can be reported.");
        var copy = await db.LibraryBookCopies.SingleAsync(x => x.Id == copyId, token);
        var member = await db.LibraryMembers.SingleAsync(x => x.Id == memberId, token);
        var policy = await Policy(member, token);
        var amount = assessedAmount ?? (condition == LibraryItemCondition.Lost ? (copy.Price ?? 0) * policy.LostBookMultiplier : (copy.Price ?? 0) * policy.DamageChargePercent / 100m);
        var incident = new LibraryItemIncident { TenantId = member.TenantId, BranchId = copy.BranchId, LibraryBookCopyId = copy.Id, LibraryMemberId = member.Id, LibraryBookIssueId = issueId, Condition = condition, Details = details, AssessedAmount = amount, Status = amount > 0 ? LibraryIncidentStatus.Assessed : LibraryIncidentStatus.Reported };
        copy.Status = condition == LibraryItemCondition.Lost ? BookCopyStatus.Lost : BookCopyStatus.Damaged;
        db.LibraryItemIncidents.Add(incident);
        await db.SaveChangesAsync(token);
        if (amount > 0)
        {
            var invoice = await CreateInvoice(member, $"LIB-INC-{incident.Id.ToString("N")[..8]}".ToUpperInvariant(), condition == LibraryItemCondition.Lost ? "Lost library book" : "Damaged library book", amount, token);
            incident.FeeInvoiceId = invoice.Id; incident.Status = LibraryIncidentStatus.Invoiced;
            await db.SaveChangesAsync(token);
        }
        return incident;
    }

    public async Task<FeeInvoice> EnsureFineInvoiceAsync(Guid fineId, CancellationToken token = default)
    {
        var fine = await db.LibraryFineRecords.Include(x => x.LibraryMember).SingleAsync(x => x.Id == fineId, token);
        if (fine.FeeInvoiceId is Guid existing) return await db.FeeInvoices.SingleAsync(x => x.Id == existing, token);
        var invoice = await CreateInvoice(fine.LibraryMember!, $"LIB-FINE-{fine.Id.ToString("N")[..8]}".ToUpperInvariant(), fine.Reason, fine.Amount, token);
        fine.FeeInvoiceId = invoice.Id;
        await db.SaveChangesAsync(token);
        return invoice;
    }

    public async Task<int> ProcessQueuesAndOverduesAsync(CancellationToken token = default)
    {
        var now = DateTime.UtcNow; var today = DateOnly.FromDateTime(now); var changes = 0;
        var expired = await db.LibraryReservations.Where(x => (x.Status == LibraryReservationStatus.Requested || x.Status == LibraryReservationStatus.Active) && x.ExpiresOn < now).ToListAsync(token);
        foreach (var reservation in expired) { reservation.Status = LibraryReservationStatus.Expired; if (reservation.LibraryBookCopyId is Guid copyId) { var copy = await db.LibraryBookCopies.SingleAsync(x => x.Id == copyId, token); copy.Status = BookCopyStatus.Available; await ActivateNextReservation(reservation.LibraryBookId, copy, token); } changes++; }
        var overdue = await db.LibraryBookIssues.Include(x => x.LibraryMember)!.ThenInclude(x => x!.StudentProfile).Include(x => x.LibraryMember)!.ThenInclude(x => x!.TeacherProfile).Where(x => x.Status == LibraryIssueStatus.Issued && x.DueDate < today).ToListAsync(token);
        foreach (var issue in overdue)
        {
            issue.Status = LibraryIssueStatus.Overdue;
            var recipient = MemberEmail(issue.LibraryMember!);
            if (!string.IsNullOrWhiteSpace(recipient) && !await db.LibraryOverdueNotifications.AnyAsync(x => x.LibraryBookIssueId == issue.Id && x.CreatedOn.Date == now.Date, token))
            {
                db.LibraryOverdueNotifications.Add(new() { TenantId = issue.TenantId, BranchId = issue.BranchId, LibraryBookIssueId = issue.Id, LibraryMemberId = issue.LibraryMemberId, Recipient = recipient, Status = NotificationStatus.Queued });
                db.NotificationMessages.Add(Message(issue.TenantId, issue.BranchId, recipient, "Library item overdue", $"Your library item was due on {issue.DueDate:dd MMM yyyy}. Please return or renew it."));
            }
            changes++;
        }
        await db.SaveChangesAsync(token); return changes;
    }

    public async Task<LibraryStockVerificationItem> VerifyStockAsync(Guid verificationId, string scanCode, CancellationToken token = default)
    {
        scanCode = scanCode.Trim();
        var verification = await db.LibraryStockVerifications.SingleAsync(x => x.Id == verificationId, token);
        var copy = await db.LibraryBookCopies.FirstOrDefaultAsync(x => x.BranchId == verification.BranchId && (x.Barcode == scanCode || x.AccessionNumber == scanCode), token);
        var item = new LibraryStockVerificationItem { TenantId = verification.TenantId, BranchId = verification.BranchId, LibraryStockVerificationId = verification.Id, LibraryBookCopyId = copy?.Id, ScanCode = scanCode, Result = copy is null ? LibraryStockResult.Unexpected : copy.Status == BookCopyStatus.Damaged ? LibraryStockResult.Damaged : LibraryStockResult.Found };
        if (copy is not null) copy.LastVerifiedOn = DateTime.UtcNow;
        verification.Status = LibraryStockVerificationStatus.InProgress;
        db.LibraryStockVerificationItems.Add(item); await db.SaveChangesAsync(token); return item;
    }

    public string CreateCode39Svg(string value, string caption)
    {
        value = $"*{new string(value.ToUpperInvariant().Where(c => char.IsLetterOrDigit(c) || "-. $/+%".Contains(c)).ToArray())}*";
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";
        string[] patterns = ["nnnwwnwnn","wnnwnnnnw","nnwwnnnnw","wnwwnnnnn","nnnwwnnnw","wnnwwnnnn","nnwwwnnnn","nnnwnnwnw","wnnwnnwnn","nnwwnnwnn","wnnnnwnnw","nnwnnwnnw","wnwnnwnnn","nnnnwwnnw","wnnnwwnnn","nnwnwwnnn","nnnnnwwnw","wnnnnwwnn","nnwnnwwnn","nnnnwwwnn","wwnnnnnnw","nwwnnnnnw","wwwnnnnnn","nwnnwnnnw","wwnnwnnnn","nwwnwnnnn","nwnnwwnnn","wwnnnnwnn","nwwnnnwnn","nwnnwnwnn","nnnnwnnww","wnnnwnnwn","nnwnwnnwn","wnwnwnnnn","nnnnwnwwn","wnnnwnwnn","nnwnwnwnn","nnnnnnwww","wnnnnnwwn","nnwnnnwwn","nwnnnnwnw","wwnnnnwnn","nwwnnnwnn","nwnnwnnwn"];
        var x = 10; var bars = new StringBuilder();
        foreach (var c in value) { var p = patterns[chars.IndexOf(c)]; for (var i = 0; i < p.Length; i++) { var w = p[i] == 'w' ? 3 : 1; if (i % 2 == 0) bars.Append($"<rect x='{x}' y='8' width='{w}' height='55'/>"); x += w; } x += 1; }
        return $"<svg xmlns='http://www.w3.org/2000/svg' width='{x + 10}' height='88' viewBox='0 0 {x + 10} 88'><rect width='100%' height='100%' fill='white'/><g fill='black'>{bars}</g><text x='{(x + 10) / 2}' y='80' text-anchor='middle' font-family='Arial' font-size='11'>{System.Net.WebUtility.HtmlEncode(caption)}</text></svg>";
    }

    private async Task<LibraryCirculationPolicy> Policy(LibraryMember member, CancellationToken token) => await db.LibraryCirculationPolicies.Where(x => x.IsActive && x.MemberType == member.MemberType && (x.BranchId == null || x.BranchId == member.BranchId)).OrderByDescending(x => x.BranchId.HasValue).FirstOrDefaultAsync(token) ?? new() { TenantId = member.TenantId, BranchId = member.BranchId, MemberType = member.MemberType, MaxIssues = member.MaxBooksAllowed };
    private async Task<FeeInvoice> CreateInvoice(LibraryMember member, string number, string description, decimal amount, CancellationToken token)
    {
        var invoice = new FeeInvoice { TenantId = member.TenantId, BranchId = member.BranchId, StudentProfileId = member.StudentProfileId, InvoiceNumber = number, InvoiceDate = DateOnly.FromDateTime(DateTime.UtcNow), DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)), Status = InvoiceStatus.Issued, SubTotal = amount, TotalAmount = amount, Lines = [new() { TenantId = member.TenantId, BranchId = member.BranchId, Description = description, ComponentType = FeeComponentType.Library, Amount = amount }] };
        db.FeeInvoices.Add(invoice); await db.SaveChangesAsync(token); await finance.PostInvoiceAsync(invoice.Id, token); return invoice;
    }
    private async Task ActivateNextReservation(Guid bookId, LibraryBookCopy copy, CancellationToken token)
    {
        var next = await db.LibraryReservations.Include(x => x.LibraryMember)!.ThenInclude(x => x!.StudentProfile).Include(x => x.LibraryMember)!.ThenInclude(x => x!.TeacherProfile).Where(x => x.LibraryBookId == bookId && x.Status == LibraryReservationStatus.Requested).OrderBy(x => x.QueuePosition).ThenBy(x => x.ReservedOn).FirstOrDefaultAsync(token);
        if (next is null) return; var policy = await Policy(next.LibraryMember!, token); next.Status = LibraryReservationStatus.Active; next.LibraryBookCopyId = copy.Id; next.ExpiresOn = DateTime.UtcNow.AddDays(policy.ReservationHoldDays); next.AvailabilityNotifiedOn = DateTime.UtcNow; copy.Status = BookCopyStatus.Reserved; await QueueAvailabilityNotice(next.LibraryMember!, bookId, next.ExpiresOn, token);
    }
    private async Task QueueAvailabilityNotice(LibraryMember member, Guid bookId, DateTime expires, CancellationToken token)
    {
        await db.Entry(member).Reference(x => x.StudentProfile).LoadAsync(token); await db.Entry(member).Reference(x => x.TeacherProfile).LoadAsync(token); var email = MemberEmail(member); if (string.IsNullOrWhiteSpace(email)) return; var title = await db.LibraryBooks.Where(x => x.Id == bookId).Select(x => x.Title).SingleAsync(token); db.NotificationMessages.Add(Message(member.TenantId, member.BranchId, email, "Reserved book available", $"{title} is ready for collection until {expires:dd MMM yyyy HH:mm}."));
    }
    private static string? MemberEmail(LibraryMember member) => member.StudentProfile?.Email ?? member.TeacherProfile?.Email ?? member.User?.Email;
    private static NotificationMessage Message(Guid tenantId, Guid branchId, string recipient, string subject, string body) => new() { TenantId = tenantId, BranchId = branchId, Channel = CommunicationChannel.Email, Subject = subject, Body = body, Status = NotificationStatus.Queued, ScheduledOn = DateTime.UtcNow, Recipients = [new() { TenantId = tenantId, BranchId = branchId, DisplayName = recipient, DestinationAddress = recipient, Status = NotificationStatus.Queued }] };
}
