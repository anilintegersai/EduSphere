namespace EduSphere.Domain.Enums;

public enum StudentLifecycleEventType
{
    ProfileCreated = 0,
    DirectAdmission = 1,
    Enrolled = 2,
    SectionTransfer = 3,
    BranchTransfer = 4,
    Promoted = 5,
    ReAdmitted = 6,
    Withdrawn = 7,
    Graduated = 8,
    Deactivated = 9,
    Reactivated = 10
}

public enum TeacherLifecycleEventType
{
    ProfileCreated = 0,
    Onboarded = 1,
    Confirmed = 2,
    BranchTransfer = 3,
    DepartmentTransfer = 4,
    SubjectAssignmentChanged = 5,
    LeaveStarted = 6,
    LeaveReturned = 7,
    Deactivated = 8,
    Relieved = 9,
    Reactivated = 10
}
