using EduSphere.Domain.Entities;

namespace EduSphere.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}
