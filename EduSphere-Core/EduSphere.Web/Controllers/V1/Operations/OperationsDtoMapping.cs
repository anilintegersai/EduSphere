using EduSphere.Application.DTOs.Operations;
using EduSphere.Domain.Common;

namespace EduSphere.Web.Controllers.V1.Operations;

internal static class OperationsDtoMapping
{
    public static T WithMetadata<T>(this T dto, TenantEntityBase entity)
        where T : OperationsTenantScopedDto
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
