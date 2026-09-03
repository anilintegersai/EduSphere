using EduSphere.Application.DTOs.Academic;
using EduSphere.Domain.Common;

namespace EduSphere.Web.Controllers.V1.Academic;

internal static class AcademicDtoMapping
{
    public static T WithMetadata<T>(this T dto, TenantEntityBase entity)
        where T : TenantScopedDto
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
