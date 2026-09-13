using System.Text.Json;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Services;

public static class AccountEmailProviderBootstrapper
{
    public static async Task ApplyConfiguredAccountEmailProviderAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AccountEmailProviderBootstrapper");
        var options = configuration.GetSection("Notifications:AccountEmailProvider").Get<AccountEmailProviderOptions>() ?? new();
        if (!options.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(options.ProviderKey) ||
            string.IsNullOrWhiteSpace(options.Host) ||
            string.IsNullOrWhiteSpace(options.FromAddress) ||
            string.IsNullOrWhiteSpace(options.UserName) ||
            string.IsNullOrWhiteSpace(options.PasswordSecretReference))
        {
            logger.LogWarning("Account email provider bootstrap is enabled, but SMTP settings are incomplete.");
            return;
        }

        var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        var tenantIds = await dbContext.Tenants
            .IgnoreQueryFilters()
            .Where(t => !t.IsDeleted && t.IsActive)
            .Select(t => t.Id)
            .ToListAsync();

        var configurationJson = JsonSerializer.Serialize(new
        {
            options.Host,
            options.Port,
            options.EnableSsl,
            options.UserName,
            options.FromAddress,
            options.FromDisplayName,
            options.IsBodyHtml
        });

        foreach (var tenantId in tenantIds)
        {
            var provider = await dbContext.NotificationProviderSettings
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.TenantId == tenantId &&
                                          p.BranchId == null &&
                                          p.Channel == CommunicationChannel.Email &&
                                          p.ProviderKey == options.ProviderKey);

            if (provider is null)
            {
                provider = new NotificationProviderSetting
                {
                    TenantId = tenantId,
                    Channel = CommunicationChannel.Email,
                    ProviderKey = options.ProviderKey
                };
                dbContext.NotificationProviderSettings.Add(provider);
            }

            provider.DisplayName = options.DisplayName;
            provider.FromAddress = options.FromAddress;
            provider.FromDisplayName = options.FromDisplayName;
            provider.ConfigurationJson = configurationJson;
            provider.SecretReference = options.PasswordSecretReference;
            provider.IsEnabled = true;
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Account email SMTP provider '{ProviderKey}' configured for {TenantCount} tenant(s).", options.ProviderKey, tenantIds.Count);
    }

    private sealed class AccountEmailProviderOptions
    {
        public bool Enabled { get; init; }
        public string ProviderKey { get; init; } = "smtp-dev";
        public string DisplayName { get; init; } = "Account email SMTP";
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; } = 587;
        public bool EnableSsl { get; init; } = true;
        public string UserName { get; init; } = string.Empty;
        public string FromAddress { get; init; } = string.Empty;
        public string? FromDisplayName { get; init; }
        public string PasswordSecretReference { get; init; } = string.Empty;
        public bool IsBodyHtml { get; init; } = true;
    }
}
