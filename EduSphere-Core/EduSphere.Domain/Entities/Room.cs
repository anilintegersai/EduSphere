using System.ComponentModel.DataAnnotations;
using EduSphere.Domain.Common;
using EduSphere.Domain.Enums;

namespace EduSphere.Domain.Entities;

public class Room : TenantEntityBase
{
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = null!;

    public RoomType Type { get; set; } = RoomType.Classroom;
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
}
