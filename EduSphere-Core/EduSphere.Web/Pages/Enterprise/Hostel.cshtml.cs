using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Pages.Enterprise;

[Authorize(Policy = AuthorizationPolicies.HostelManager)]
public class HostelModel : EnterprisePageModel
{
    private readonly ICrudService<HostelBlock> _blocks;
    private readonly ICrudService<HostelRoom> _rooms;
    private readonly ICrudService<HostelBed> _beds;
    private readonly ICrudService<HostelAllocation> _allocations;
    private readonly ICrudService<HostelFee> _fees;
    private readonly ICrudService<HostelVisitorLog> _visitorLogs;
    private readonly ICrudService<HostelMaintenanceRequest> _maintenanceRequests;
    private readonly ICrudService<HostelAllocationTransferRequest> _transferRequests;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;

    public HostelModel(
        ICrudService<HostelBlock> blocks,
        ICrudService<HostelRoom> rooms,
        ICrudService<HostelBed> beds,
        ICrudService<HostelAllocation> allocations,
        ICrudService<HostelFee> fees,
        ICrudService<HostelVisitorLog> visitorLogs,
        ICrudService<HostelMaintenanceRequest> maintenanceRequests,
        ICrudService<HostelAllocationTransferRequest> transferRequests,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
        : base(tenant, branchAccess)
    {
        _blocks = blocks;
        _rooms = rooms;
        _beds = beds;
        _allocations = allocations;
        _fees = fees;
        _visitorLogs = visitorLogs;
        _maintenanceRequests = maintenanceRequests;
        _transferRequests = transferRequests;
        _branches = branches;
        _students = students;
    }

    public IReadOnlyList<HostelBlock> Blocks { get; private set; } = new List<HostelBlock>();
    public IReadOnlyList<HostelRoom> Rooms { get; private set; } = new List<HostelRoom>();
    public IReadOnlyList<HostelBed> Beds { get; private set; } = new List<HostelBed>();
    public IReadOnlyList<HostelAllocation> Allocations { get; private set; } = new List<HostelAllocation>();
    public IReadOnlyList<HostelFee> Fees { get; private set; } = new List<HostelFee>();
    public IReadOnlyList<HostelVisitorLog> VisitorLogs { get; private set; } = new List<HostelVisitorLog>();
    public IReadOnlyList<HostelMaintenanceRequest> MaintenanceRequests { get; private set; } = new List<HostelMaintenanceRequest>();
    public IReadOnlyList<HostelAllocationTransferRequest> TransferRequests { get; private set; } = new List<HostelAllocationTransferRequest>();
    public IReadOnlyList<StudentProfile> Students { get; private set; } = new List<StudentProfile>();

    [BindProperty] public BlockInputModel BlockInput { get; set; } = new();
    [BindProperty] public RoomInputModel RoomInput { get; set; } = new();
    [BindProperty] public BedInputModel BedInput { get; set; } = new();
    [BindProperty] public AllocationInputModel AllocationInput { get; set; } = new();
    [BindProperty] public FeeInputModel FeeInput { get; set; } = new();
    [BindProperty] public VisitorInputModel VisitorInput { get; set; } = new();
    [BindProperty] public MaintenanceRequestInputModel MaintenanceRequestInput { get; set; } = new();
    [BindProperty] public TransferInputModel TransferInput { get; set; } = new();

    public class BlockInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(40)] public string Code { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.NotSpecified;
        [StringLength(120), Display(Name = "Warden")] public string? WardenName { get; set; }
        [StringLength(30), Display(Name = "Warden phone")] public string? WardenPhone { get; set; }
    }

    public class RoomInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Block")] public Guid? HostelBlockId { get; set; }
        [Required, StringLength(40), Display(Name = "Room #")] public string RoomNumber { get; set; } = string.Empty;
        [StringLength(40)] public string? Floor { get; set; }
        [Range(1, 20)] public int Capacity { get; set; } = 2;
        [Range(0, 100000), Display(Name = "Monthly fee")] public decimal MonthlyFee { get; set; }
        public HostelRoomStatus Status { get; set; } = HostelRoomStatus.Available;
    }

    public class BedInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Room")] public Guid? HostelRoomId { get; set; }
        [Required, StringLength(40), Display(Name = "Bed #")] public string BedNumber { get; set; } = string.Empty;
        public HostelBedStatus Status { get; set; } = HostelBedStatus.Available;
    }

    public class AllocationInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, Display(Name = "Room")] public Guid? HostelRoomId { get; set; }
        [Display(Name = "Bed")] public Guid? HostelBedId { get; set; }
        [Display(Name = "Allocated on")] public DateOnly AllocatedOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Range(0, 100000), Display(Name = "Monthly fee")] public decimal MonthlyFee { get; set; }
        [Range(0, 100000), Display(Name = "Security deposit")] public decimal SecurityDeposit { get; set; }
        public HostelAllocationStatus Status { get; set; } = HostelAllocationStatus.Active;
    }

    public class FeeInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Allocation")] public Guid? HostelAllocationId { get; set; }
        [Required, StringLength(60), Display(Name = "Invoice #")] public string InvoiceNumber { get; set; } = string.Empty;
        [Display(Name = "Billing month")] public DateOnly BillingMonth { get; set; } = new(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        [Display(Name = "Due date")] public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        [Range(0, 100000)] public decimal Amount { get; set; }
        [Range(0, 100000), Display(Name = "Paid")] public decimal PaidAmount { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Issued;
    }

    public class VisitorInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Allocation")] public Guid? HostelAllocationId { get; set; }
        [Display(Name = "Student")] public Guid? StudentProfileId { get; set; }
        [Required, StringLength(120), Display(Name = "Visitor")] public string VisitorName { get; set; } = string.Empty;
        [StringLength(80)] public string? Relationship { get; set; }
        [Required, StringLength(30), Display(Name = "Phone")] public string PhoneNumber { get; set; } = string.Empty;
        [StringLength(250)] public string? Purpose { get; set; }
        [StringLength(80), Display(Name = "ID document")] public string? IdDocumentNumber { get; set; }
    }

    public class MaintenanceRequestInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Display(Name = "Block")] public Guid? HostelBlockId { get; set; }
        [Display(Name = "Room")] public Guid? HostelRoomId { get; set; }
        [Display(Name = "Bed")] public Guid? HostelBedId { get; set; }
        [Required, StringLength(120)] public string Category { get; set; } = "General";
        public HostelMaintenancePriority Priority { get; set; } = HostelMaintenancePriority.Medium;
        public HostelMaintenanceStatus Status { get; set; } = HostelMaintenanceStatus.Open;
        [Display(Name = "Due on")] public DateTime? DueOn { get; set; }
        [Required, StringLength(1000)] public string Description { get; set; } = string.Empty;
    }

    public class TransferInputModel
    {
        [Required, Display(Name = "Branch")] public Guid? BranchId { get; set; }
        [Required, Display(Name = "Allocation")] public Guid? HostelAllocationId { get; set; }
        [Required, Display(Name = "To room")] public Guid? ToRoomId { get; set; }
        [Display(Name = "To bed")] public Guid? ToBedId { get; set; }
        [Display(Name = "Effective on")] public DateOnly EffectiveOn { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Required, StringLength(500)] public string Reason { get; set; } = string.Empty;
    }

    public string BlockName(Guid id) => Blocks.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string RoomName(Guid id) => Rooms.FirstOrDefault(r => r.Id == id)?.RoomNumber ?? "-";
    public string BedName(Guid? id) => Beds.FirstOrDefault(b => b.Id == id)?.BedNumber ?? "-";
    public string StudentName(Guid id)
    {
        var student = Students.FirstOrDefault(s => s.Id == id);
        return student is null ? "-" : $"{student.FirstName} {student.LastName}";
    }

    public string AllocationName(Guid id)
    {
        var allocation = Allocations.FirstOrDefault(a => a.Id == id);
        return allocation is null ? "-" : $"{StudentName(allocation.StudentProfileId)} / {RoomName(allocation.HostelRoomId)}";
    }

    public async Task OnGetAsync()
    {
        if (!HasTenant) return;
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveBlockAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(BlockInput));
        if (!await ValidateBranchSelectionAsync("BlockInput.BranchId", BlockInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _blocks.CreateAsync(new HostelBlock
        {
            BranchId = BlockInput.BranchId!.Value,
            Name = BlockInput.Name,
            Code = BlockInput.Code,
            Gender = BlockInput.Gender,
            WardenName = BlockInput.WardenName,
            WardenPhone = BlockInput.WardenPhone
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveRoomAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(RoomInput));
        if (!await ValidateBranchSelectionAsync("RoomInput.BranchId", RoomInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (RoomInput.HostelBlockId is not Guid blockId || await _blocks.GetAsync(blockId) is not { } block || block.BranchId != RoomInput.BranchId)
            ModelState.AddModelError("RoomInput.HostelBlockId", "Selected block was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _rooms.CreateAsync(new HostelRoom
        {
            BranchId = RoomInput.BranchId!.Value,
            HostelBlockId = RoomInput.HostelBlockId!.Value,
            RoomNumber = RoomInput.RoomNumber,
            Floor = RoomInput.Floor,
            Capacity = RoomInput.Capacity,
            MonthlyFee = RoomInput.MonthlyFee,
            Status = RoomInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveBedAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(BedInput));
        if (!await ValidateBranchSelectionAsync("BedInput.BranchId", BedInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (BedInput.HostelRoomId is not Guid roomId || await _rooms.GetAsync(roomId) is not { } room || room.BranchId != BedInput.BranchId)
            ModelState.AddModelError("BedInput.HostelRoomId", "Selected room was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _beds.CreateAsync(new HostelBed
        {
            BranchId = BedInput.BranchId!.Value,
            HostelRoomId = BedInput.HostelRoomId!.Value,
            BedNumber = BedInput.BedNumber,
            Status = BedInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveAllocationAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(AllocationInput));
        if (!await ValidateBranchSelectionAsync("AllocationInput.BranchId", AllocationInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (AllocationInput.StudentProfileId is not Guid studentId || await _students.GetAsync(studentId) is not { } student || student.BranchId != AllocationInput.BranchId)
            ModelState.AddModelError("AllocationInput.StudentProfileId", "Selected student was not found for this branch.");
        if (AllocationInput.HostelRoomId is not Guid roomId || await _rooms.GetAsync(roomId) is not { } room || room.BranchId != AllocationInput.BranchId)
            ModelState.AddModelError("AllocationInput.HostelRoomId", "Selected room was not found for this branch.");
        if (AllocationInput.HostelBedId is Guid bedId && (await _beds.GetAsync(bedId) is not { } bed || bed.BranchId != AllocationInput.BranchId || bed.HostelRoomId != AllocationInput.HostelRoomId))
            ModelState.AddModelError("AllocationInput.HostelBedId", "Selected bed was not found for this room.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _allocations.CreateAsync(new HostelAllocation
        {
            BranchId = AllocationInput.BranchId!.Value,
            StudentProfileId = AllocationInput.StudentProfileId!.Value,
            HostelRoomId = AllocationInput.HostelRoomId!.Value,
            HostelBedId = AllocationInput.HostelBedId,
            AllocatedOn = AllocationInput.AllocatedOn,
            MonthlyFee = AllocationInput.MonthlyFee,
            SecurityDeposit = AllocationInput.SecurityDeposit,
            Status = AllocationInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveFeeAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(FeeInput));
        if (!await ValidateBranchSelectionAsync("FeeInput.BranchId", FeeInput.BranchId, _branches)) ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (FeeInput.HostelAllocationId is not Guid allocationId || await _allocations.GetAsync(allocationId) is not { } allocation || allocation.BranchId != FeeInput.BranchId)
            ModelState.AddModelError("FeeInput.HostelAllocationId", "Selected allocation was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }
        await _fees.CreateAsync(new HostelFee
        {
            BranchId = FeeInput.BranchId!.Value,
            HostelAllocationId = FeeInput.HostelAllocationId!.Value,
            InvoiceNumber = FeeInput.InvoiceNumber,
            BillingMonth = FeeInput.BillingMonth,
            DueDate = FeeInput.DueDate,
            Amount = FeeInput.Amount,
            PaidAmount = FeeInput.PaidAmount,
            Status = FeeInput.Status
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteBlockAsync(Guid id) { await DeleteIfAllowedAsync(_blocks, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteRoomAsync(Guid id) { await DeleteIfAllowedAsync(_rooms, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteBedAsync(Guid id) { await DeleteIfAllowedAsync(_beds, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteAllocationAsync(Guid id) { await DeleteIfAllowedAsync(_allocations, id, x => x.BranchId); return RedirectToPage(); }
    public async Task<IActionResult> OnPostDeleteFeeAsync(Guid id) { await DeleteIfAllowedAsync(_fees, id, x => x.BranchId); return RedirectToPage(); }

    public async Task<IActionResult> OnPostSaveVisitorAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(VisitorInput));
        if (!await ValidateBranchSelectionAsync("VisitorInput.BranchId", VisitorInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (VisitorInput.HostelAllocationId is Guid allocationId && (await _allocations.GetAsync(allocationId) is not { } allocation || allocation.BranchId != VisitorInput.BranchId))
            ModelState.AddModelError("VisitorInput.HostelAllocationId", "Selected allocation was not found for this branch.");
        if (VisitorInput.StudentProfileId is Guid studentId && (await _students.GetAsync(studentId) is not { } student || student.BranchId != VisitorInput.BranchId))
            ModelState.AddModelError("VisitorInput.StudentProfileId", "Selected student was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _visitorLogs.CreateAsync(new HostelVisitorLog
        {
            BranchId = VisitorInput.BranchId!.Value,
            HostelAllocationId = VisitorInput.HostelAllocationId,
            StudentProfileId = VisitorInput.StudentProfileId,
            VisitorName = VisitorInput.VisitorName,
            Relationship = VisitorInput.Relationship,
            PhoneNumber = VisitorInput.PhoneNumber,
            Purpose = VisitorInput.Purpose,
            IdDocumentNumber = VisitorInput.IdDocumentNumber
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveMaintenanceRequestAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(MaintenanceRequestInput));
        if (!await ValidateBranchSelectionAsync("MaintenanceRequestInput.BranchId", MaintenanceRequestInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        if (MaintenanceRequestInput.HostelBlockId is Guid blockId && (await _blocks.GetAsync(blockId) is not { } block || block.BranchId != MaintenanceRequestInput.BranchId))
            ModelState.AddModelError("MaintenanceRequestInput.HostelBlockId", "Selected block was not found for this branch.");
        if (MaintenanceRequestInput.HostelRoomId is Guid roomId && (await _rooms.GetAsync(roomId) is not { } room || room.BranchId != MaintenanceRequestInput.BranchId))
            ModelState.AddModelError("MaintenanceRequestInput.HostelRoomId", "Selected room was not found for this branch.");
        if (MaintenanceRequestInput.HostelBedId is Guid bedId && (await _beds.GetAsync(bedId) is not { } bed || bed.BranchId != MaintenanceRequestInput.BranchId))
            ModelState.AddModelError("MaintenanceRequestInput.HostelBedId", "Selected bed was not found for this branch.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _maintenanceRequests.CreateAsync(new HostelMaintenanceRequest
        {
            BranchId = MaintenanceRequestInput.BranchId!.Value,
            HostelBlockId = MaintenanceRequestInput.HostelBlockId,
            HostelRoomId = MaintenanceRequestInput.HostelRoomId,
            HostelBedId = MaintenanceRequestInput.HostelBedId,
            Category = MaintenanceRequestInput.Category,
            Priority = MaintenanceRequestInput.Priority,
            Status = MaintenanceRequestInput.Status,
            DueOn = MaintenanceRequestInput.DueOn,
            Description = MaintenanceRequestInput.Description
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRequestTransferAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(TransferInput));
        if (!await ValidateBranchSelectionAsync("TransferInput.BranchId", TransferInput.BranchId, _branches))
            ModelState.AddModelError(string.Empty, "Fix branch selection.");
        HostelAllocation? allocation = null;
        if (TransferInput.HostelAllocationId is not Guid allocationId || await _allocations.GetAsync(allocationId) is not { } selectedAllocation || selectedAllocation.BranchId != TransferInput.BranchId)
            ModelState.AddModelError("TransferInput.HostelAllocationId", "Selected allocation was not found for this branch.");
        else
            allocation = selectedAllocation;
        if (TransferInput.ToRoomId is not Guid roomId || await _rooms.GetAsync(roomId) is not { } room || room.BranchId != TransferInput.BranchId)
            ModelState.AddModelError("TransferInput.ToRoomId", "Selected destination room was not found for this branch.");
        if (TransferInput.ToBedId is Guid bedId && (await _beds.GetAsync(bedId) is not { } bed || bed.BranchId != TransferInput.BranchId || bed.HostelRoomId != TransferInput.ToRoomId))
            ModelState.AddModelError("TransferInput.ToBedId", "Selected destination bed was not found for this room.");
        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        await _transferRequests.CreateAsync(new HostelAllocationTransferRequest
        {
            BranchId = TransferInput.BranchId!.Value,
            HostelAllocationId = TransferInput.HostelAllocationId!.Value,
            FromRoomId = allocation!.HostelRoomId,
            FromBedId = allocation.HostelBedId,
            ToRoomId = TransferInput.ToRoomId!.Value,
            ToBedId = TransferInput.ToBedId,
            EffectiveOn = TransferInput.EffectiveOn,
            Reason = TransferInput.Reason,
            Status = HostelTransferStatus.Requested
        });
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateTransferStatusAsync(Guid id, HostelTransferStatus status, string? decisionNotes)
    {
        var request = await _transferRequests.GetAsync(id);
        if (request is not null && await CanUseBranchAsync(request.BranchId))
        {
            await _transferRequests.UpdateAsync(id, r =>
            {
                r.Status = status;
                r.DecisionNotes = decisionNotes;
                if (status == HostelTransferStatus.Approved)
                    r.ApprovedOn = DateTime.UtcNow;
            });

            if (status == HostelTransferStatus.Completed)
            {
                await _allocations.UpdateAsync(request.HostelAllocationId, a =>
                {
                    a.HostelRoomId = request.ToRoomId;
                    a.HostelBedId = request.ToBedId;
                });
                if (request.FromBedId is Guid fromBedId)
                    await _beds.UpdateAsync(fromBedId, b => b.Status = HostelBedStatus.Available);
                if (request.ToBedId is Guid toBedId)
                    await _beds.UpdateAsync(toBedId, b => b.Status = HostelBedStatus.Occupied);
            }
        }
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        await LoadBranchesAsync(_branches);
        Blocks = (await FilterBranchScopedAsync(_blocks, x => x.BranchId)).OrderBy(b => b.Name).ToList();
        Rooms = (await FilterBranchScopedAsync(_rooms, x => x.BranchId)).OrderBy(r => r.RoomNumber).ToList();
        Beds = (await FilterBranchScopedAsync(_beds, x => x.BranchId)).OrderBy(b => b.BedNumber).ToList();
        Allocations = (await FilterBranchScopedAsync(_allocations, x => x.BranchId)).OrderByDescending(a => a.AllocatedOn).ToList();
        Fees = (await FilterBranchScopedAsync(_fees, x => x.BranchId)).OrderByDescending(f => f.BillingMonth).ToList();
        VisitorLogs = (await FilterBranchScopedAsync(_visitorLogs, x => x.BranchId)).OrderByDescending(v => v.CheckInOn).ToList();
        MaintenanceRequests = (await FilterBranchScopedAsync(_maintenanceRequests, x => x.BranchId)).OrderByDescending(m => m.ReportedOn).ToList();
        TransferRequests = (await FilterBranchScopedAsync(_transferRequests, x => x.BranchId)).OrderByDescending(t => t.RequestedOn).ToList();
        Students = (await FilterBranchScopedAsync(_students, x => x.BranchId)).OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ToList();

        if (await DefaultBranchIdAsync() is Guid branchId)
        {
            BlockInput.BranchId ??= branchId;
            RoomInput.BranchId ??= branchId;
            BedInput.BranchId ??= branchId;
            AllocationInput.BranchId ??= branchId;
            FeeInput.BranchId ??= branchId;
            VisitorInput.BranchId ??= branchId;
            MaintenanceRequestInput.BranchId ??= branchId;
            TransferInput.BranchId ??= branchId;
        }
    }

    private async Task DeleteIfAllowedAsync<T>(ICrudService<T> service, Guid id, Func<T, Guid?> branchSelector)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        if (entity is not null && await CanUseBranchAsync(branchSelector(entity)))
            await service.SoftDeleteAsync(id);
    }
}
