using EduSphere.Application.DTOs.Examinations;
using EduSphere.Domain.Common;

namespace EduSphere.Web.Controllers.V1.Examinations;

internal static class ExaminationDtoMapping
{
    public static T WithMetadata<T>(this T dto, TenantEntityBase entity)
        where T : ExaminationTenantScopedDto
    {
        dto.Id = entity.Id;
        dto.TenantId = entity.TenantId;
        dto.IsDeleted = entity.IsDeleted;
        dto.CreatedBy = entity.CreatedBy;
        dto.CreatedOn = entity.CreatedOn;
        dto.ModifiedBy = entity.ModifiedBy;
        dto.ModifiedOn = entity.ModifiedOn;
        dto.DeletedBy = entity.DeletedBy;
        dto.DeletedOn = entity.DeletedOn;
        dto.ConcurrencyToken = entity.ConcurrencyToken;
        return dto;
    }
}
