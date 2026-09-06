using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/library")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.LibraryManager)]
public class LibraryController : EnterpriseControllerBase
{
    private readonly ICrudService<LibraryBook> _books;
    private readonly ICrudService<LibraryBookCopy> _copies;
    private readonly ICrudService<LibraryMember> _members;
    private readonly ICrudService<LibraryBookIssue> _issues;
    private readonly ICrudService<LibraryBookReturn> _returns;
    private readonly ICrudService<LibraryFineRecord> _fines;
    private readonly ICrudService<Branch> _branches;

    public LibraryController(
        ICrudService<LibraryBook> books,
        ICrudService<LibraryBookCopy> copies,
        ICrudService<LibraryMember> members,
        ICrudService<LibraryBookIssue> issues,
        ICrudService<LibraryBookReturn> returns,
        ICrudService<LibraryFineRecord> fines,
        ICrudService<Branch> branches,
        IBranchAccessService branchAccess)
        : base(branchAccess)
    {
        _books = books;
        _copies = copies;
        _members = members;
        _issues = issues;
        _returns = returns;
        _fines = fines;
        _branches = branches;
    }

    [HttpGet("books")]
    public async Task<IActionResult> GetBooks([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryBookDto>>.Ok((await FilterByBranchAsync(_books, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("books")]
    public async Task<IActionResult> CreateBook([FromBody] CreateLibraryBookRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _books.CreateAsync(Apply(new LibraryBook(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryBookDto>.Ok(created.Map()));
    }

    [HttpPut("books/{id:guid}")]
    public async Task<IActionResult> UpdateBook(Guid id, [FromBody] UpdateLibraryBookRequest request)
    {
        if (await GetBranchScopedAsync(_books, id, x => x.BranchId) is null) return NotFoundResult(id, "Book");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _books.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("books/{id:guid}")]
    public Task<IActionResult> DeleteBook(Guid id)
        => DeleteBranchScopedAsync(_books, id, x => x.BranchId, "Book");

    [HttpGet("copies")]
    public async Task<IActionResult> GetCopies([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryBookCopyDto>>.Ok((await FilterByBranchAsync(_copies, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("copies")]
    public async Task<IActionResult> CreateCopy([FromBody] CreateLibraryBookCopyRequest request)
    {
        if (await ValidateBookChildAsync(request.LibraryBookId, request.BranchId) is { } invalid) return invalid;
        var created = await _copies.CreateAsync(Apply(new LibraryBookCopy(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryBookCopyDto>.Ok(created.Map()));
    }

    [HttpPut("copies/{id:guid}")]
    public async Task<IActionResult> UpdateCopy(Guid id, [FromBody] UpdateLibraryBookCopyRequest request)
    {
        if (await GetBranchScopedAsync(_copies, id, x => x.BranchId) is null) return NotFoundResult(id, "Book copy");
        if (await ValidateBookChildAsync(request.LibraryBookId, request.BranchId) is { } invalid) return invalid;
        await _copies.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("copies/{id:guid}")]
    public Task<IActionResult> DeleteCopy(Guid id)
        => DeleteBranchScopedAsync(_copies, id, x => x.BranchId, "Book copy");

    [HttpGet("members")]
    public async Task<IActionResult> GetMembers([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryMemberDto>>.Ok((await FilterByBranchAsync(_members, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("members")]
    public async Task<IActionResult> CreateMember([FromBody] CreateLibraryMemberRequest request)
    {
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _members.CreateAsync(Apply(new LibraryMember(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryMemberDto>.Ok(created.Map()));
    }

    [HttpPut("members/{id:guid}")]
    public async Task<IActionResult> UpdateMember(Guid id, [FromBody] UpdateLibraryMemberRequest request)
    {
        if (await GetBranchScopedAsync(_members, id, x => x.BranchId) is null) return NotFoundResult(id, "Library member");
        if (await ValidateBranchAsync(request.BranchId) is { } invalid) return invalid;
        await _members.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("members/{id:guid}")]
    public Task<IActionResult> DeleteMember(Guid id)
        => DeleteBranchScopedAsync(_members, id, x => x.BranchId, "Library member");

    [HttpGet("issues")]
    public async Task<IActionResult> GetIssues([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryBookIssueDto>>.Ok((await FilterByBranchAsync(_issues, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("issues")]
    public async Task<IActionResult> CreateIssue([FromBody] CreateLibraryBookIssueRequest request)
    {
        if (await ValidateIssueAsync(request.LibraryBookCopyId, request.LibraryMemberId, request.BranchId) is { } invalid) return invalid;
        var created = await _issues.CreateAsync(Apply(new LibraryBookIssue(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryBookIssueDto>.Ok(created.Map()));
    }

    [HttpPut("issues/{id:guid}")]
    public async Task<IActionResult> UpdateIssue(Guid id, [FromBody] UpdateLibraryBookIssueRequest request)
    {
        if (await GetBranchScopedAsync(_issues, id, x => x.BranchId) is null) return NotFoundResult(id, "Book issue");
        if (await ValidateIssueAsync(request.LibraryBookCopyId, request.LibraryMemberId, request.BranchId) is { } invalid) return invalid;
        await _issues.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("issues/{id:guid}")]
    public Task<IActionResult> DeleteIssue(Guid id)
        => DeleteBranchScopedAsync(_issues, id, x => x.BranchId, "Book issue");

    [HttpGet("returns")]
    public async Task<IActionResult> GetReturns([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryBookReturnDto>>.Ok((await FilterByBranchAsync(_returns, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("returns")]
    public async Task<IActionResult> CreateReturn([FromBody] CreateLibraryBookReturnRequest request)
    {
        if (await ValidateIssueChildAsync(request.LibraryBookIssueId, request.BranchId) is { } invalid) return invalid;
        var created = await _returns.CreateAsync(Apply(new LibraryBookReturn(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryBookReturnDto>.Ok(created.Map()));
    }

    [HttpPut("returns/{id:guid}")]
    public async Task<IActionResult> UpdateReturn(Guid id, [FromBody] UpdateLibraryBookReturnRequest request)
    {
        if (await GetBranchScopedAsync(_returns, id, x => x.BranchId) is null) return NotFoundResult(id, "Book return");
        if (await ValidateIssueChildAsync(request.LibraryBookIssueId, request.BranchId) is { } invalid) return invalid;
        await _returns.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("returns/{id:guid}")]
    public Task<IActionResult> DeleteReturn(Guid id)
        => DeleteBranchScopedAsync(_returns, id, x => x.BranchId, "Book return");

    [HttpGet("fines")]
    public async Task<IActionResult> GetFines([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<LibraryFineRecordDto>>.Ok((await FilterByBranchAsync(_fines, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("fines")]
    public async Task<IActionResult> CreateFine([FromBody] CreateLibraryFineRecordRequest request)
    {
        if (await ValidateFineAsync(request.LibraryMemberId, request.LibraryBookIssueId, request.BranchId) is { } invalid) return invalid;
        var created = await _fines.CreateAsync(Apply(new LibraryFineRecord(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LibraryFineRecordDto>.Ok(created.Map()));
    }

    [HttpPut("fines/{id:guid}")]
    public async Task<IActionResult> UpdateFine(Guid id, [FromBody] UpdateLibraryFineRecordRequest request)
    {
        if (await GetBranchScopedAsync(_fines, id, x => x.BranchId) is null) return NotFoundResult(id, "Library fine");
        if (await ValidateFineAsync(request.LibraryMemberId, request.LibraryBookIssueId, request.BranchId) is { } invalid) return invalid;
        await _fines.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("fines/{id:guid}")]
    public Task<IActionResult> DeleteFine(Guid id)
        => DeleteBranchScopedAsync(_fines, id, x => x.BranchId, "Library fine");

    private async Task<IActionResult?> ValidateBranchAsync(Guid branchId)
    {
        if (await _branches.GetAsync(branchId) is null)
            return BadRequest(ApiResponse<object>.Fail("Selected branch was not found in this tenant."));
        return await CanUseBranchAsync(branchId) ? null : Forbid();
    }

    private async Task<IActionResult?> ValidateBookChildAsync(Guid bookId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _books.GetAsync(bookId) is not { } book || book.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected book was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateIssueAsync(Guid copyId, Guid memberId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _copies.GetAsync(copyId) is not { } copy || copy.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected book copy was not found for the branch."));
        if (await _members.GetAsync(memberId) is not { } member || member.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected member was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateIssueChildAsync(Guid issueId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _issues.GetAsync(issueId) is not { } issue || issue.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected issue was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateFineAsync(Guid memberId, Guid? issueId, Guid branchId)
    {
        if (await ValidateBranchAsync(branchId) is { } invalid) return invalid;
        if (await _members.GetAsync(memberId) is not { } member || member.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected member was not found for the branch."));
        if (issueId is Guid checkedIssueId && (await _issues.GetAsync(checkedIssueId) is not { } issue || issue.BranchId != branchId))
            return BadRequest(ApiResponse<object>.Fail("Selected issue was not found for the branch."));
        return null;
    }

    private NotFoundObjectResult NotFoundResult(Guid id, string displayName)
        => NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

    private static LibraryBook Apply(LibraryBook entity, CreateLibraryBookRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.SubjectId = request.SubjectId;
        entity.Isbn = request.Isbn;
        entity.Title = request.Title;
        entity.Authors = request.Authors;
        entity.Publisher = request.Publisher;
        entity.Category = request.Category;
        entity.Edition = request.Edition;
        entity.PublicationYear = request.PublicationYear;
        entity.Language = request.Language;
        entity.Keywords = request.Keywords;
        return entity;
    }

    private static LibraryBookCopy Apply(LibraryBookCopy entity, CreateLibraryBookCopyRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.LibraryBookId = request.LibraryBookId;
        entity.AccessionNumber = request.AccessionNumber;
        entity.Barcode = request.Barcode;
        entity.ShelfLocation = request.ShelfLocation;
        entity.Status = request.Status;
        entity.AcquisitionDate = request.AcquisitionDate;
        entity.Price = request.Price;
        return entity;
    }

    private static LibraryMember Apply(LibraryMember entity, CreateLibraryMemberRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.TeacherProfileId = request.TeacherProfileId;
        entity.UserId = request.UserId;
        entity.MemberNumber = request.MemberNumber;
        entity.MemberType = request.MemberType;
        entity.JoinedOn = request.JoinedOn;
        entity.ExpiresOn = request.ExpiresOn;
        entity.MaxBooksAllowed = request.MaxBooksAllowed;
        entity.IsActive = request.IsActive;
        entity.Notes = request.Notes;
        return entity;
    }

    private static LibraryBookIssue Apply(LibraryBookIssue entity, CreateLibraryBookIssueRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.LibraryBookCopyId = request.LibraryBookCopyId;
        entity.LibraryMemberId = request.LibraryMemberId;
        entity.IssueDate = request.IssueDate;
        entity.DueDate = request.DueDate;
        entity.ReturnedOn = request.ReturnedOn;
        entity.Status = request.Status;
        entity.FineAmount = request.FineAmount;
        entity.IssuedByUserId = request.IssuedByUserId;
        entity.ReturnReceivedByUserId = request.ReturnReceivedByUserId;
        entity.Notes = request.Notes;
        return entity;
    }

    private static LibraryBookReturn Apply(LibraryBookReturn entity, CreateLibraryBookReturnRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.LibraryBookIssueId = request.LibraryBookIssueId;
        entity.ReturnedOn = request.ReturnedOn;
        entity.FineAssessed = request.FineAssessed;
        entity.FinePaid = request.FinePaid;
        entity.ReceivedByUserId = request.ReceivedByUserId;
        entity.ConditionNotes = request.ConditionNotes;
        entity.Notes = request.Notes;
        return entity;
    }

    private static LibraryFineRecord Apply(LibraryFineRecord entity, CreateLibraryFineRecordRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.LibraryMemberId = request.LibraryMemberId;
        entity.LibraryBookIssueId = request.LibraryBookIssueId;
        entity.Amount = request.Amount;
        entity.Reason = request.Reason;
        entity.Status = request.Status;
        entity.AssessedOn = request.AssessedOn;
        entity.PaidOn = request.PaidOn;
        entity.WaivedOn = request.WaivedOn;
        entity.Notes = request.Notes;
        return entity;
    }
}
