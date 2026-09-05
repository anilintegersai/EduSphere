using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

/// <summary>
/// Verifies the shared-DB / shared-schema tenancy model: the global query filter
/// isolates reads, the ChangeTracker stamps TenantId on writes, and an unresolved
/// tenant sees no tenant-owned rows.
/// </summary>
public class TenantIsolationTests
{
    private static readonly Guid Tenant1Id = Guid.Parse("11111111-aaaa-4111-8111-111111111111");
    private static readonly Guid Tenant2Id = Guid.Parse("22222222-bbbb-4222-8222-222222222222");
    private static readonly Guid Tenant7Id = Guid.Parse("77777777-cccc-4777-8777-777777777777");
    private static readonly Guid Tenant100Id = Guid.Parse("aaaaaaaa-dddd-4aaa-8aaa-aaaaaaaaaaaa");
    private static readonly Guid Tenant200Id = Guid.Parse("bbbbbbbb-eeee-4bbb-8bbb-bbbbbbbbbbbb");

    private static TenantDbContext NewDb(string dbName, TenantContext tenantContext, TestCurrentUserContext? currentUser = null)
    {
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new TenantDbContext(options, tenantContext, currentUser);
    }

    [Fact]
    public void GlobalQueryFilter_IsolatesReadsByTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        // Seed two tenants' branches (writes are not tenant-filtered).
        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - North", TenantId = Tenant1Id });
            db.Branches.Add(new Branch { Name = "Tenant1 - South", TenantId = Tenant1Id });
            db.Branches.Add(new Branch { Name = "Tenant2 - Main", TenantId = Tenant2Id });
            db.SaveChanges();
        }

        // Read as tenant 1.
        var t1 = new TenantContext();
        t1.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            var branches = db.Branches.ToList();
            Assert.Equal(2, branches.Count);
            Assert.All(branches, b => Assert.Equal(Tenant1Id, b.TenantId));
        }

        // Read as tenant 2.
        var t2 = new TenantContext();
        t2.SetTenant(Tenant2Id, "tenant2");
        using (var db = NewDb(dbName, t2))
        {
            var branch = Assert.Single(db.Branches.ToList());
            Assert.Equal("Tenant2 - Main", branch.Name);
        }
    }

    [Fact]
    public void SaveChanges_StampsCurrentTenantOnInsert()
    {
        var dbName = Guid.NewGuid().ToString();
        var ctx = new TenantContext();
        ctx.SetTenant(Tenant7Id, "tenant7");

        using var db = NewDb(dbName, ctx);
        var branch = new Branch { Name = "Unstamped" }; // TenantId left empty
        db.Branches.Add(branch);
        db.SaveChanges();

        Assert.Equal(Tenant7Id, branch.TenantId);
    }

    [Fact]
    public void NoResolvedTenant_ReturnsNoTenantOwnedRows()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Tenant1 - Only", TenantId = Tenant1Id });
            db.SaveChanges();
        }

        // A request with no tenant resolved must not see tenant-owned data.
        using (var db = NewDb(dbName, new TenantContext()))
        {
            Assert.Empty(db.Branches.ToList());
        }
    }

    [Fact]
    public void AcademicEntities_AreTenantFilteredAndStamped()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Courses.Add(new Course { Code = "A", Name = "Course A", TenantId = Tenant1Id });
            db.Courses.Add(new Course { Code = "B", Name = "Course B", TenantId = Tenant2Id });
            db.SaveChanges();
        }

        var t1 = new TenantContext();
        t1.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, t1))
        {
            // Read: filtered to tenant 1 only.
            var course = Assert.Single(db.Courses.ToList());
            Assert.Equal(Tenant1Id, course.TenantId);

            // Write: TenantId is stamped from the current tenant when left unset.
            var created = new Course { Code = "C", Name = "Course C" };
            db.Courses.Add(created);
            db.SaveChanges();
            Assert.Equal(Tenant1Id, created.TenantId);
        }
    }

    [Fact]
    public void CrossTenantData_IsNotVisibleToAnotherTenant()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.Branches.Add(new Branch { Name = "Secret", TenantId = Tenant100Id });
            db.SaveChanges();
        }

        var other = new TenantContext();
        other.SetTenant(Tenant200Id, "tenant200");
        using (var db = NewDb(dbName, other))
        {
            Assert.Empty(db.Branches.ToList());
        }
    }

    [Fact]
    public void SaveChanges_StampsGuidAuditAndConcurrencyToken()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        var currentUser = new TestCurrentUserContext { UserId = "creator-user" };

        using var db = NewDb(dbName, tenant, currentUser);
        var branch = new Branch { Name = "Audited" };
        db.Branches.Add(branch);
        db.SaveChanges();

        Assert.NotEqual(Guid.Empty, branch.Id);
        Assert.Equal(Tenant1Id, branch.TenantId);
        Assert.Equal("creator-user", branch.CreatedBy);
        Assert.NotEqual(default, branch.CreatedOn);
        Assert.Null(branch.ModifiedBy);
        Assert.Null(branch.ModifiedOn);
        Assert.NotEqual(Guid.Empty, branch.ConcurrencyToken);

        var originalToken = branch.ConcurrencyToken;
        currentUser.UserId = "editor-user";
        branch.Name = "Audited Updated";
        db.SaveChanges();

        Assert.Equal("creator-user", branch.CreatedBy);
        Assert.Equal("editor-user", branch.ModifiedBy);
        Assert.NotNull(branch.ModifiedOn);
        Assert.NotEqual(originalToken, branch.ConcurrencyToken);
    }

    [Fact]
    public void SoftDelete_HidesRowsAndStampsDeleteMetadata()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        var currentUser = new TestCurrentUserContext { UserId = "deleter-user" };

        using var db = NewDb(dbName, tenant, currentUser);
        var branch = new Branch { Name = "Archived" };
        db.Branches.Add(branch);
        db.SaveChanges();

        db.Branches.Remove(branch);
        db.SaveChanges();

        Assert.Empty(db.Branches.ToList());

        var deleted = Assert.Single(db.Branches.IgnoreQueryFilters().ToList());
        Assert.True(deleted.IsDeleted);
        Assert.Equal("deleter-user", deleted.DeletedBy);
        Assert.NotNull(deleted.DeletedOn);
    }

    [Fact]
    public void StudentAndTeacherProfiles_AreTenantFilteredAndStamped()
    {
        var dbName = Guid.NewGuid().ToString();
        var branchId = Guid.NewGuid();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.StudentProfiles.Add(CreateStudent("S-100", Tenant1Id, branchId));
            db.StudentProfiles.Add(CreateStudent("S-200", Tenant2Id, branchId));
            db.TeacherProfiles.Add(CreateTeacher("T-100", Tenant1Id, branchId));
            db.TeacherProfiles.Add(CreateTeacher("T-200", Tenant2Id, branchId));
            db.SaveChanges();
        }

        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, tenant))
        {
            var student = Assert.Single(db.StudentProfiles.ToList());
            var teacher = Assert.Single(db.TeacherProfiles.ToList());
            Assert.Equal("S-100", student.AdmissionNumber);
            Assert.Equal("T-100", teacher.EmployeeNumber);

            var createdStudent = CreateStudent("S-101", Guid.Empty, branchId);
            var createdTeacher = CreateTeacher("T-101", Guid.Empty, branchId);
            db.StudentProfiles.Add(createdStudent);
            db.TeacherProfiles.Add(createdTeacher);
            db.SaveChanges();

            Assert.Equal(Tenant1Id, createdStudent.TenantId);
            Assert.Equal(Tenant1Id, createdTeacher.TenantId);
            Assert.NotEqual(Guid.Empty, createdStudent.ConcurrencyToken);
            Assert.NotEqual(Guid.Empty, createdTeacher.ConcurrencyToken);
        }
    }

    [Fact]
    public void PeopleSupportRecords_AreTenantFilteredAndStamped()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");

        using var db = NewDb(dbName, tenant);
        db.StudentGuardians.Add(new StudentGuardian
        {
            StudentProfileId = Guid.NewGuid(),
            FullName = "Primary Guardian",
            Relationship = GuardianRelationship.Guardian,
            IsPrimary = true
        });
        db.TeacherSubjectAssignments.Add(new TeacherSubjectAssignment
        {
            TeacherProfileId = Guid.NewGuid(),
            SubjectId = Guid.NewGuid(),
            IsPrimary = true
        });
        db.ProfileDocuments.Add(new ProfileDocument
        {
            OwnerType = ProfileDocumentOwnerType.Student,
            OwnerId = Guid.NewGuid(),
            DocumentType = "Identity",
            DisplayName = "Student ID",
            FileName = "student-id.pdf",
            StoragePath = "students/student-id.pdf"
        });
        db.SaveChanges();

        Assert.Single(db.StudentGuardians.ToList());
        Assert.Single(db.TeacherSubjectAssignments.ToList());
        Assert.Single(db.ProfileDocuments.ToList());
        Assert.All(db.ProfileDocuments.ToList(), d => Assert.Equal(Tenant1Id, d.TenantId));

        using var noTenantDb = NewDb(dbName, new TenantContext());
        Assert.Empty(noTenantDb.StudentGuardians.ToList());
        Assert.Empty(noTenantDb.TeacherSubjectAssignments.ToList());
        Assert.Empty(noTenantDb.ProfileDocuments.ToList());
    }

    [Fact]
    public void OperationsEntities_AreTenantFilteredAndStamped()
    {
        var dbName = Guid.NewGuid().ToString();
        var branchId = Guid.NewGuid();
        var sectionId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var academicYearId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var timetableId = Guid.NewGuid();
        var timeSlotId = Guid.NewGuid();

        using (var db = NewDb(dbName, new TenantContext()))
        {
            db.AdmissionApplications.Add(CreateAdmissionApplication("APP-100", Tenant1Id, branchId, courseId));
            db.AdmissionApplications.Add(CreateAdmissionApplication("APP-200", Tenant2Id, branchId, courseId));
            db.AttendanceSessions.Add(new AttendanceSession
            {
                Id = sessionId,
                BranchId = branchId,
                SectionId = sectionId,
                TenantId = Tenant1Id
            });
            db.AttendanceSessions.Add(new AttendanceSession
            {
                BranchId = branchId,
                SectionId = sectionId,
                TenantId = Tenant2Id
            });
            db.AttendanceRecords.Add(new AttendanceRecord
            {
                AttendanceSessionId = sessionId,
                StudentProfileId = studentId,
                TenantId = Tenant1Id
            });
            db.AttendanceRecords.Add(new AttendanceRecord
            {
                AttendanceSessionId = Guid.NewGuid(),
                StudentProfileId = Guid.NewGuid(),
                TenantId = Tenant2Id
            });
            db.Rooms.Add(new Room { BranchId = branchId, Code = "R-100", Name = "Room 100", TenantId = Tenant1Id });
            db.Rooms.Add(new Room { BranchId = branchId, Code = "R-200", Name = "Room 200", TenantId = Tenant2Id });
            db.TimeSlots.Add(new TimeSlot
            {
                Id = timeSlotId,
                BranchId = branchId,
                Name = "Period 1",
                DayOfWeek = DayOfWeek.Monday,
                PeriodNumber = 1,
                StartsAt = new TimeOnly(8, 0),
                EndsAt = new TimeOnly(8, 45),
                TenantId = Tenant1Id
            });
            db.TimeSlots.Add(new TimeSlot
            {
                BranchId = branchId,
                Name = "Period 2",
                DayOfWeek = DayOfWeek.Monday,
                PeriodNumber = 2,
                StartsAt = new TimeOnly(9, 0),
                EndsAt = new TimeOnly(9, 45),
                TenantId = Tenant2Id
            });
            db.Timetables.Add(new Timetable
            {
                Id = timetableId,
                BranchId = branchId,
                AcademicYearId = academicYearId,
                SectionId = sectionId,
                Name = "Tenant 1 Timetable",
                TenantId = Tenant1Id
            });
            db.Timetables.Add(new Timetable
            {
                BranchId = branchId,
                AcademicYearId = academicYearId,
                SectionId = Guid.NewGuid(),
                Name = "Tenant 2 Timetable",
                TenantId = Tenant2Id
            });
            db.TimetableEntries.Add(new TimetableEntry
            {
                TimetableId = timetableId,
                SectionId = sectionId,
                SubjectId = subjectId,
                TeacherProfileId = teacherId,
                TimeSlotId = timeSlotId,
                TenantId = Tenant1Id
            });
            db.TimetableEntries.Add(new TimetableEntry
            {
                TimetableId = Guid.NewGuid(),
                SectionId = Guid.NewGuid(),
                SubjectId = Guid.NewGuid(),
                TeacherProfileId = Guid.NewGuid(),
                TimeSlotId = Guid.NewGuid(),
                TenantId = Tenant2Id
            });
            db.SaveChanges();
        }

        var tenant = new TenantContext();
        tenant.SetTenant(Tenant1Id, "tenant1");
        using (var db = NewDb(dbName, tenant))
        {
            Assert.Equal("APP-100", Assert.Single(db.AdmissionApplications.ToList()).ApplicationNumber);
            Assert.Equal(sessionId, Assert.Single(db.AttendanceSessions.ToList()).Id);
            Assert.Equal(studentId, Assert.Single(db.AttendanceRecords.ToList()).StudentProfileId);
            Assert.Equal("R-100", Assert.Single(db.Rooms.ToList()).Code);
            Assert.Equal(timeSlotId, Assert.Single(db.TimeSlots.ToList()).Id);
            Assert.Equal(timetableId, Assert.Single(db.Timetables.ToList()).Id);
            Assert.Equal(subjectId, Assert.Single(db.TimetableEntries.ToList()).SubjectId);

            var createdAdmission = CreateAdmissionApplication("APP-101", Guid.Empty, branchId, courseId);
            var createdSession = new AttendanceSession { BranchId = branchId, SectionId = sectionId };
            var createdRoom = new Room { BranchId = branchId, Code = "R-101", Name = "Room 101" };
            var createdSlot = new TimeSlot
            {
                BranchId = branchId,
                Name = "Period 3",
                DayOfWeek = DayOfWeek.Tuesday,
                PeriodNumber = 3,
                StartsAt = new TimeOnly(10, 0),
                EndsAt = new TimeOnly(10, 45)
            };
            var createdTimetable = new Timetable
            {
                BranchId = branchId,
                AcademicYearId = academicYearId,
                SectionId = sectionId,
                Name = "Tenant 1 Draft"
            };

            db.AdmissionApplications.Add(createdAdmission);
            db.AttendanceSessions.Add(createdSession);
            db.Rooms.Add(createdRoom);
            db.TimeSlots.Add(createdSlot);
            db.Timetables.Add(createdTimetable);
            db.SaveChanges();

            var createdEntities = new TenantEntityBase[]
            {
                createdAdmission,
                createdSession,
                createdRoom,
                createdSlot,
                createdTimetable
            };

            Assert.All(createdEntities, entity =>
            {
                Assert.Equal(Tenant1Id, entity.TenantId);
                Assert.NotEqual(Guid.Empty, entity.ConcurrencyToken);
            });
        }
    }

    private static StudentProfile CreateStudent(string admissionNumber, Guid tenantId, Guid branchId)
    {
        return new StudentProfile
        {
            AdmissionNumber = admissionNumber,
            FirstName = "Student",
            LastName = admissionNumber,
            DateOfBirth = new DateOnly(2012, 1, 1),
            AdmissionDate = new DateOnly(2026, 6, 1),
            BranchId = branchId,
            TenantId = tenantId
        };
    }

    private static TeacherProfile CreateTeacher(string employeeNumber, Guid tenantId, Guid branchId)
    {
        return new TeacherProfile
        {
            EmployeeNumber = employeeNumber,
            FirstName = "Teacher",
            LastName = employeeNumber,
            JoiningDate = new DateOnly(2026, 6, 1),
            BranchId = branchId,
            TenantId = tenantId
        };
    }

    private static AdmissionApplication CreateAdmissionApplication(
        string applicationNumber,
        Guid tenantId,
        Guid branchId,
        Guid courseId)
    {
        return new AdmissionApplication
        {
            ApplicationNumber = applicationNumber,
            ApplicantFirstName = "Applicant",
            ApplicantLastName = applicationNumber,
            DateOfBirth = new DateOnly(2015, 1, 1),
            BranchId = branchId,
            CourseId = courseId,
            TenantId = tenantId
        };
    }

    private sealed class TestCurrentUserContext : ICurrentUserContext
    {
        public string? UserId { get; set; }
    }
}
