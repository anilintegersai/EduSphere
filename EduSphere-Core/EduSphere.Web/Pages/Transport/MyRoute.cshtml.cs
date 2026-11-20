using System.Security.Claims;
using EduSphere.Domain.Constants;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Transport;

[Authorize(Roles=Roles.Student+","+Roles.Parent)]
public sealed class MyRouteModel(TenantDbContext db):PageModel
{
    public IReadOnlyList<StudentTransportAssignment> Assignments{get;private set;}=[];public IReadOnlyList<TransportRouteStop> Stops{get;private set;}=[];public IReadOnlyList<VehicleGpsPing> Positions{get;private set;}=[];public IReadOnlyList<TransportAttendanceScan> Scans{get;private set;}=[];public IReadOnlyList<TransportGeofenceEvent> Arrivals{get;private set;}=[];
    public async Task OnGetAsync(){var uid=Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);var studentIds=User.IsInRole(Roles.Parent)?await db.StudentGuardians.Where(x=>x.ParentUserId==uid&&x.HasPortalAccess).Select(x=>x.StudentProfileId).ToListAsync():await db.StudentProfiles.Where(x=>x.UserId==uid).Select(x=>x.Id).ToListAsync();Assignments=await db.StudentTransportAssignments.Include(x=>x.StudentProfile).Include(x=>x.TransportRoute).Include(x=>x.TransportRouteStop).Include(x=>x.Vehicle).Where(x=>studentIds.Contains(x.StudentProfileId)&&x.Status==Domain.Enums.TransportAssignmentStatus.Active).ToListAsync();var routeIds=Assignments.Select(x=>x.TransportRouteId).Distinct().ToList();var vehicleIds=Assignments.Where(x=>x.VehicleId.HasValue).Select(x=>x.VehicleId!.Value).Distinct().ToList();Stops=await db.TransportRouteStops.Where(x=>routeIds.Contains(x.TransportRouteId)).OrderBy(x=>x.StopOrder).ToListAsync();Positions=(await db.VehicleGpsPings.Where(x=>vehicleIds.Contains(x.VehicleId)).OrderByDescending(x=>x.RecordedOn).Take(100).ToListAsync()).GroupBy(x=>x.VehicleId).Select(x=>x.First()).ToList();Scans=await db.TransportAttendanceScans.Where(x=>studentIds.Contains(x.StudentProfileId)).OrderByDescending(x=>x.ScannedOn).Take(30).ToListAsync();Arrivals=await db.TransportGeofenceEvents.Where(x=>vehicleIds.Contains(x.VehicleId)).OrderByDescending(x=>x.OccurredOn).Take(20).ToListAsync();}
}
