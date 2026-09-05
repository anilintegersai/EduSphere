using System.ComponentModel.DataAnnotations;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Domain.MultiTenancy;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainTimetable = EduSphere.Domain.Entities.Timetable;

namespace EduSphere.Web.Pages.Timetable;

[Authorize(Policy = AuthorizationPolicies.BranchAdmin)]
public class ScheduleModel : PageModel
{
    private readonly ICrudService<Room> _rooms;
    private readonly ICrudService<TimeSlot> _slots;
    private readonly ICrudService<DomainTimetable> _timetables;
    private readonly ICrudService<TimetableEntry> _entries;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<AcademicYear> _years;
    private readonly ICrudService<Section> _sections;
    private readonly ICrudService<Subject> _subjects;
    private readonly ICrudService<TeacherProfile> _teachers;
    private readonly ITenantContext _tenant;
    private readonly IBranchAccessService _branchAccess;

    public ScheduleModel(
        ICrudService<Room> rooms,
        ICrudService<TimeSlot> slots,
        ICrudService<DomainTimetable> timetables,
        ICrudService<TimetableEntry> entries,
        ICrudService<Branch> branches,
        ICrudService<AcademicYear> years,
        ICrudService<Section> sections,
        ICrudService<Subject> subjects,
        ICrudService<TeacherProfile> teachers,
        ITenantContext tenant,
        IBranchAccessService branchAccess)
    {
        _rooms = rooms;
        _slots = slots;
        _timetables = timetables;
        _entries = entries;
        _branches = branches;
        _years = years;
        _sections = sections;
        _subjects = subjects;
        _teachers = teachers;
        _tenant = tenant;
        _branchAccess = branchAccess;
    }

    public bool HasTenant => _tenant.HasTenant;
    public bool IsBranchAdminOnly => _branchAccess.IsBranchAdminOnly(User);
    public IReadOnlyList<Room> Rooms { get; private set; } = new List<Room>();
    public IReadOnlyList<TimeSlot> TimeSlots { get; private set; } = new List<TimeSlot>();
    public IReadOnlyList<DomainTimetable> Timetables { get; private set; } = new List<DomainTimetable>();
    public IReadOnlyList<TimetableEntry> Entries { get; private set; } = new List<TimetableEntry>();
    public IReadOnlyList<Branch> Branches { get; private set; } = new List<Branch>();
    public IReadOnlyList<AcademicYear> Years { get; private set; } = new List<AcademicYear>();
    public IReadOnlyList<Section> Sections { get; private set; } = new List<Section>();
    public IReadOnlyList<Subject> Subjects { get; private set; } = new List<Subject>();
    public IReadOnlyList<TeacherProfile> Teachers { get; private set; } = new List<TeacherProfile>();

    [BindProperty] public RoomInputModel RoomInput { get; set; } = new();
    [BindProperty] public SlotInputModel SlotInput { get; set; } = new();
    [BindProperty] public TimetableInputModel TimetableInput { get; set; } = new();
    [BindProperty] public EntryInputModel EntryInput { get; set; } = new();

    public bool IsEditingRoom => RoomInput.Id != Guid.Empty;
    public bool IsEditingSlot => SlotInput.Id != Guid.Empty;
    public bool IsEditingTimetable => TimetableInput.Id != Guid.Empty;
    public bool IsEditingEntry => EntryInput.Id != Guid.Empty;

    public string BranchName(Guid id) => Branches.FirstOrDefault(b => b.Id == id)?.Name ?? "-";
    public string YearName(Guid id) => Years.FirstOrDefault(y => y.Id == id)?.Name ?? "-";
    public string SectionName(Guid id) => Sections.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string SubjectName(Guid id) => Subjects.FirstOrDefault(s => s.Id == id)?.Name ?? "-";
    public string TeacherName(Guid id)
    {
        var teacher = Teachers.FirstOrDefault(t => t.Id == id);
        return teacher is null ? "-" : $"{teacher.FirstName} {teacher.LastName}";
    }
    public string SlotName(Guid id)
    {
        var slot = TimeSlots.FirstOrDefault(s => s.Id == id);
        return slot is null ? "-" : $"{slot.DayOfWeek} P{slot.PeriodNumber} {slot.StartsAt:HH\\:mm}-{slot.EndsAt:HH\\:mm}";
    }
    public string RoomName(Guid? id) => Rooms.FirstOrDefault(r => r.Id == id)?.Name ?? "-";
    public string TimetableName(Guid id) => Timetables.FirstOrDefault(t => t.Id == id)?.Name ?? "-";

    public class RoomInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        public RoomType Type { get; set; } = RoomType.Classroom;
        [Range(0, 100000)] public int Capacity { get; set; } = 30;
        [Display(Name = "Active")] public bool IsActive { get; set; } = true;
    }

    public class SlotInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
        [Display(Name = "Day")] public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;
        [Range(1, 50), Display(Name = "Period")] public int PeriodNumber { get; set; } = 1;
        [Required, Display(Name = "Start")] public TimeOnly StartsAt { get; set; } = new(9, 0);
        [Required, Display(Name = "End")] public TimeOnly EndsAt { get; set; } = new(9, 45);
        [Display(Name = "Break")] public bool IsBreak { get; set; }
    }

    public class TimetableInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Branch"), Required] public Guid? BranchId { get; set; }
        [Display(Name = "Academic year"), Required] public Guid? AcademicYearId { get; set; }
        [Display(Name = "Section"), Required] public Guid? SectionId { get; set; }
        [Required, StringLength(120)] public string Name { get; set; } = string.Empty;
        [Required, Display(Name = "Effective from")] public DateOnly EffectiveFrom { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Display(Name = "Effective to")] public DateOnly? EffectiveTo { get; set; }
        public TimetableStatus Status { get; set; } = TimetableStatus.Draft;
    }

    public class EntryInputModel
    {
        public Guid Id { get; set; }
        [Display(Name = "Timetable"), Required] public Guid? TimetableId { get; set; }
        [Display(Name = "Section"), Required] public Guid? SectionId { get; set; }
        [Display(Name = "Subject"), Required] public Guid? SubjectId { get; set; }
        [Display(Name = "Teacher"), Required] public Guid? TeacherProfileId { get; set; }
        [Display(Name = "Time slot"), Required] public Guid? TimeSlotId { get; set; }
        [Display(Name = "Room")] public Guid? RoomId { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
    }

    public async Task OnGetAsync(Guid? roomEditId, Guid? slotEditId, Guid? timetableEditId, Guid? entryEditId)
    {
        if (!HasTenant) return;
        await LoadAsync();
        if (roomEditId is Guid roomId && await _rooms.GetAsync(roomId) is { } room && await CanUseBranchAsync(room.BranchId))
            RoomInput = Map(room);
        if (slotEditId is Guid slotId && await _slots.GetAsync(slotId) is { } slot && await CanUseBranchAsync(slot.BranchId))
            SlotInput = Map(slot);
        if (timetableEditId is Guid timetableId && await _timetables.GetAsync(timetableId) is { } timetable && await CanUseBranchAsync(timetable.BranchId))
            TimetableInput = Map(timetable);
        if (entryEditId is Guid entryId &&
            await _entries.GetAsync(entryId) is { } entry &&
            await _timetables.GetAsync(entry.TimetableId) is { } entryTimetable &&
            await CanUseBranchAsync(entryTimetable.BranchId))
        {
            EntryInput = Map(entry);
        }
    }

    public async Task<IActionResult> OnPostSaveRoomAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(RoomInput));
        var branchId = RoomInput.BranchId ?? Guid.Empty;
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("RoomInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("RoomInput.BranchId", "You can manage rooms only for your assigned branch.");
        if (RoomInput.Id != Guid.Empty && await _rooms.GetAsync(RoomInput.Id) is { } existingRoom && !await CanUseBranchAsync(existingRoom.BranchId))
            ModelState.AddModelError(string.Empty, "You can update rooms only in your assigned branch.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (RoomInput.Id == Guid.Empty)
            await _rooms.CreateAsync(Apply(new Room(), branchId));
        else
            await _rooms.UpdateAsync(RoomInput.Id, e => Apply(e, branchId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveSlotAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(SlotInput));
        var branchId = SlotInput.BranchId ?? Guid.Empty;
        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("SlotInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("SlotInput.BranchId", "You can manage time slots only for your assigned branch.");
        if (SlotInput.EndsAt <= SlotInput.StartsAt)
            ModelState.AddModelError("SlotInput.EndsAt", "End time must be after start time.");
        if (SlotInput.Id != Guid.Empty && await _slots.GetAsync(SlotInput.Id) is { } existingSlot && !await CanUseBranchAsync(existingSlot.BranchId))
            ModelState.AddModelError(string.Empty, "You can update time slots only in your assigned branch.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (SlotInput.Id == Guid.Empty)
            await _slots.CreateAsync(Apply(new TimeSlot(), branchId));
        else
            await _slots.UpdateAsync(SlotInput.Id, e => Apply(e, branchId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveTimetableAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(TimetableInput));
        var branchId = TimetableInput.BranchId ?? Guid.Empty;
        var yearId = TimetableInput.AcademicYearId ?? Guid.Empty;
        var sectionId = TimetableInput.SectionId ?? Guid.Empty;

        if (branchId == Guid.Empty || await _branches.GetAsync(branchId) is null)
            ModelState.AddModelError("TimetableInput.BranchId", "Selected branch was not found.");
        else if (!await CanUseBranchAsync(branchId))
            ModelState.AddModelError("TimetableInput.BranchId", "You can manage timetables only for your assigned branch.");
        if (yearId == Guid.Empty || await _years.GetAsync(yearId) is null)
            ModelState.AddModelError("TimetableInput.AcademicYearId", "Selected academic year was not found.");
        if (sectionId == Guid.Empty || await _sections.GetAsync(sectionId) is null)
            ModelState.AddModelError("TimetableInput.SectionId", "Selected section was not found.");
        if (TimetableInput.EffectiveTo.HasValue && TimetableInput.EffectiveTo < TimetableInput.EffectiveFrom)
            ModelState.AddModelError("TimetableInput.EffectiveTo", "Effective to must be on or after effective from.");
        if (TimetableInput.Id != Guid.Empty && await _timetables.GetAsync(TimetableInput.Id) is { } existingTimetable && !await CanUseBranchAsync(existingTimetable.BranchId))
            ModelState.AddModelError(string.Empty, "You can update timetables only in your assigned branch.");

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        if (TimetableInput.Id == Guid.Empty)
            await _timetables.CreateAsync(Apply(new DomainTimetable(), branchId, yearId, sectionId));
        else
            await _timetables.UpdateAsync(TimetableInput.Id, e => Apply(e, branchId, yearId, sectionId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSaveEntryAsync()
    {
        if (!HasTenant) return RedirectToPage();
        KeepOnlyModelStateFor(nameof(EntryInput));
        var validationError = await ValidateEntryAsync(EntryInput, EntryInput.Id == Guid.Empty ? null : EntryInput.Id);
        if (validationError is not null)
            ModelState.AddModelError(string.Empty, validationError);

        if (!ModelState.IsValid) { await LoadAsync(); return Page(); }

        var timetableId = EntryInput.TimetableId!.Value;
        var sectionId = EntryInput.SectionId!.Value;
        var subjectId = EntryInput.SubjectId!.Value;
        var teacherId = EntryInput.TeacherProfileId!.Value;
        var slotId = EntryInput.TimeSlotId!.Value;

        if (EntryInput.Id == Guid.Empty)
            await _entries.CreateAsync(Apply(new TimetableEntry(), timetableId, sectionId, subjectId, teacherId, slotId));
        else
            await _entries.UpdateAsync(EntryInput.Id, e => Apply(e, timetableId, sectionId, subjectId, teacherId, slotId));

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteRoomAsync(Guid id)
    {
        if (await _rooms.GetAsync(id) is { } room && await CanUseBranchAsync(room.BranchId))
            await _rooms.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteSlotAsync(Guid id)
    {
        if (await _slots.GetAsync(id) is { } slot && await CanUseBranchAsync(slot.BranchId))
            await _slots.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteTimetableAsync(Guid id)
    {
        if (await _timetables.GetAsync(id) is { } timetable && await CanUseBranchAsync(timetable.BranchId))
            await _timetables.SoftDeleteAsync(id);

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteEntryAsync(Guid id)
    {
        if (await _entries.GetAsync(id) is { } entry &&
            await _timetables.GetAsync(entry.TimetableId) is { } timetable &&
            await CanUseBranchAsync(timetable.BranchId))
        {
            await _entries.SoftDeleteAsync(id);
        }

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var assignedBranchId = await _branchAccess.GetAssignedBranchIdAsync(User);
        Branches = await _branchAccess.FilterBranchesAsync(User, await _branches.ListAsync());
        Years = (await _years.ListAsync()).OrderByDescending(y => y.StartDate).ToList();
        Sections = (await _sections.ListAsync()).OrderBy(s => s.Name).ToList();
        Subjects = (await _subjects.ListAsync()).OrderBy(s => s.Name).ToList();
        Teachers = (await _teachers.ListAsync(t =>
                !IsBranchAdminOnly ||
                (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)))
            .OrderBy(t => t.FirstName)
            .ThenBy(t => t.LastName)
            .ToList();
        Rooms = (await _rooms.ListAsync(r =>
                !IsBranchAdminOnly ||
                (assignedBranchId.HasValue && r.BranchId == assignedBranchId.Value)))
            .OrderBy(r => r.Code)
            .ToList();
        TimeSlots = (await _slots.ListAsync(s =>
                !IsBranchAdminOnly ||
                (assignedBranchId.HasValue && s.BranchId == assignedBranchId.Value)))
            .OrderBy(s => s.DayOfWeek)
            .ThenBy(s => s.PeriodNumber)
            .ToList();
        Timetables = (await _timetables.ListAsync(t =>
                !IsBranchAdminOnly ||
                (assignedBranchId.HasValue && t.BranchId == assignedBranchId.Value)))
            .OrderByDescending(t => t.EffectiveFrom)
            .ThenBy(t => t.Name)
            .ToList();
        var timetableIds = Timetables.Select(t => t.Id).ToHashSet();
        Entries = (await _entries.ListAsync(e => !IsBranchAdminOnly || timetableIds.Contains(e.TimetableId)))
            .OrderBy(e => TimetableName(e.TimetableId))
            .ThenBy(e => SlotName(e.TimeSlotId))
            .ToList();

        if (IsBranchAdminOnly && assignedBranchId.HasValue)
        {
            RoomInput.BranchId ??= assignedBranchId.Value;
            SlotInput.BranchId ??= assignedBranchId.Value;
            TimetableInput.BranchId ??= assignedBranchId.Value;
        }
    }

    private void KeepOnlyModelStateFor(string prefix)
    {
        foreach (var key in ModelState.Keys.Where(k => !k.StartsWith(prefix + ".", StringComparison.Ordinal)).ToList())
            ModelState.Remove(key);
    }

    private async Task<string?> ValidateEntryAsync(EntryInputModel input, Guid? currentId)
    {
        if (input.TimetableId is not Guid timetableId || input.SectionId is not Guid sectionId ||
            input.SubjectId is not Guid subjectId || input.TeacherProfileId is not Guid teacherId ||
            input.TimeSlotId is not Guid slotId)
        {
            return "Timetable, section, subject, teacher, and time slot are required.";
        }

        var timetable = await _timetables.GetAsync(timetableId);
        if (timetable is null)
            return "Selected timetable was not found.";
        if (!await CanUseBranchAsync(timetable.BranchId))
            return "You can manage timetable entries only for your assigned branch.";
        if (await _sections.GetAsync(sectionId) is null)
            return "Selected section was not found.";
        if (await _subjects.GetAsync(subjectId) is null)
            return "Selected subject was not found.";
        var teacher = await _teachers.GetAsync(teacherId);
        if (teacher is null)
            return "Selected teacher was not found.";
        if (teacher.BranchId != timetable.BranchId)
            return "Selected teacher must belong to the timetable branch.";
        var slot = await _slots.GetAsync(slotId);
        if (slot is null)
            return "Selected time slot was not found.";
        if (slot.BranchId != timetable.BranchId)
            return "Selected time slot must belong to the timetable branch.";
        if (input.RoomId is Guid roomId)
        {
            var room = await _rooms.GetAsync(roomId);
            if (room is null)
                return "Selected room was not found.";
            if (room.BranchId != timetable.BranchId)
                return "Selected room must belong to the timetable branch.";
        }
        if (timetable.SectionId != sectionId)
            return "Entry section must match the selected timetable's section.";

        var collisions = await _entries.ListAsync(e =>
            e.TimetableId == timetableId &&
            e.TimeSlotId == slotId &&
            (!currentId.HasValue || e.Id != currentId.Value));

        if (collisions.Any(e => e.SectionId == sectionId))
            return "The section already has a class in this time slot.";
        if (collisions.Any(e => e.TeacherProfileId == teacherId))
            return "The teacher already has a class in this time slot.";
        if (input.RoomId.HasValue && collisions.Any(e => e.RoomId == input.RoomId))
            return "The room already has a class in this time slot.";

        return null;
    }

    private Task<bool> CanUseBranchAsync(Guid branchId) => _branchAccess.CanAccessBranchAsync(User, branchId);

    private Room Apply(Room entity, Guid branchId)
    {
        entity.BranchId = branchId;
        entity.Code = RoomInput.Code;
        entity.Name = RoomInput.Name;
        entity.Type = RoomInput.Type;
        entity.Capacity = RoomInput.Capacity;
        entity.IsActive = RoomInput.IsActive;
        return entity;
    }

    private TimeSlot Apply(TimeSlot entity, Guid branchId)
    {
        entity.BranchId = branchId;
        entity.Name = SlotInput.Name;
        entity.DayOfWeek = SlotInput.DayOfWeek;
        entity.PeriodNumber = SlotInput.PeriodNumber;
        entity.StartsAt = SlotInput.StartsAt;
        entity.EndsAt = SlotInput.EndsAt;
        entity.IsBreak = SlotInput.IsBreak;
        return entity;
    }

    private DomainTimetable Apply(DomainTimetable entity, Guid branchId, Guid yearId, Guid sectionId)
    {
        entity.BranchId = branchId;
        entity.AcademicYearId = yearId;
        entity.SectionId = sectionId;
        entity.Name = TimetableInput.Name;
        entity.EffectiveFrom = TimetableInput.EffectiveFrom;
        entity.EffectiveTo = TimetableInput.EffectiveTo;
        entity.Status = TimetableInput.Status;
        return entity;
    }

    private TimetableEntry Apply(TimetableEntry entity, Guid timetableId, Guid sectionId, Guid subjectId, Guid teacherId, Guid slotId)
    {
        entity.TimetableId = timetableId;
        entity.SectionId = sectionId;
        entity.SubjectId = subjectId;
        entity.TeacherProfileId = teacherId;
        entity.TimeSlotId = slotId;
        entity.RoomId = EntryInput.RoomId;
        entity.Notes = EntryInput.Notes;
        return entity;
    }

    private static RoomInputModel Map(Room e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        Code = e.Code,
        Name = e.Name,
        Type = e.Type,
        Capacity = e.Capacity,
        IsActive = e.IsActive
    };

    private static SlotInputModel Map(TimeSlot e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        Name = e.Name,
        DayOfWeek = e.DayOfWeek,
        PeriodNumber = e.PeriodNumber,
        StartsAt = e.StartsAt,
        EndsAt = e.EndsAt,
        IsBreak = e.IsBreak
    };

    private static TimetableInputModel Map(DomainTimetable e) => new()
    {
        Id = e.Id,
        BranchId = e.BranchId,
        AcademicYearId = e.AcademicYearId,
        SectionId = e.SectionId,
        Name = e.Name,
        EffectiveFrom = e.EffectiveFrom,
        EffectiveTo = e.EffectiveTo,
        Status = e.Status
    };

    private static EntryInputModel Map(TimetableEntry e) => new()
    {
        Id = e.Id,
        TimetableId = e.TimetableId,
        SectionId = e.SectionId,
        SubjectId = e.SubjectId,
        TeacherProfileId = e.TeacherProfileId,
        TimeSlotId = e.TimeSlotId,
        RoomId = e.RoomId,
        Notes = e.Notes
    };
}
