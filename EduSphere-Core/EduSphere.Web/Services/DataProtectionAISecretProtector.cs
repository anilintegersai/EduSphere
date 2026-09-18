using EduSphere.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace EduSphere.Web.Services;

public sealed class DataProtectionAISecretProtector : IAISecretProtector
{
    private readonly IDataProtector _protector;
    public DataProtectionAISecretProtector(IDataProtectionProvider provider) =>
        _protector = provider.CreateProtector("EduSphere.AI.ProviderSecrets.v1");
    public string Protect(string secret) => _protector.Protect(secret);
    public string Unprotect(string protectedSecret) => _protector.Unprotect(protectedSecret);
}
