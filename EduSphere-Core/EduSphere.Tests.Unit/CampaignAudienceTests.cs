using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Infrastructure;
using EduSphere.Infrastructure.MultiTenancy;
using EduSphere.Web.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EduSphere.Tests.Unit;

public sealed class CampaignAudienceTests
{
    private sealed class User : ICurrentUserContext { public string? UserId => "campaign-tests"; }

    private static (TenantDbContext Db, Guid TenantId) Create()
    {
        var tenantId = Guid.NewGuid();
        var tenant = new TenantContext();
        tenant.SetTenant(tenantId, "campaign-tests");
        var db = new TenantDbContext(
            new DbContextOptionsBuilder<TenantDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
            tenant,
            new User());
        return (db, tenantId);
    }

    private static ApplicationUser Person(Guid tenantId, string name, string phone) => new()
    {
        Id = Guid.NewGuid(), TenantId = tenantId, UserName = $"{name}@example.test", Email = $"{name}@example.test",
        PhoneNumber = phone, FirstName = name, LastName = "Test", IsActive = true
    };

    [Fact]
    public async Task SkipsUsersWhoChoseNeverOrDisabledTheChannel()
    {
        var x = Create();
        await using var db = x.Db;
        var never = Person(x.TenantId, "never", "9000000001");
        var disabled = Person(x.TenantId, "disabled", "9000000002");
        var digest = Person(x.TenantId, "digest", "9000000003");
        var none = Person(x.TenantId, "none", "9000000004");
        db.AddRange(
            new UserCommunicationPreference { TenantId = x.TenantId, UserId = never.Id, Channel = CommunicationChannel.Email, Frequency = CommunicationPreferenceFrequency.Never },
            new UserCommunicationPreference { TenantId = x.TenantId, UserId = disabled.Id, Channel = CommunicationChannel.Email, IsEnabled = false },
            new UserCommunicationPreference { TenantId = x.TenantId, UserId = digest.Id, Channel = CommunicationChannel.Email, Frequency = CommunicationPreferenceFrequency.WeeklyDigest });
        await db.SaveChangesAsync();

        var result = await CampaignAudience.ExcludeOptedOutAsync(db, [never, disabled, digest, none], CommunicationChannel.Email);

        Assert.Equal(new[] { digest.Id, none.Id }, result.Select(u => u.Id));
    }

    [Fact]
    public async Task SkipsActiveUnsubscribesByAccountOrAddressOnThatChannelOnly()
    {
        var x = Create();
        await using var db = x.Db;
        var byAccount = Person(x.TenantId, "account", "9000000011");
        var byAddress = Person(x.TenantId, "address", "9000000012");
        var resubscribed = Person(x.TenantId, "resubscribed", "9000000013");
        var otherChannel = Person(x.TenantId, "other", "9000000014");
        db.AddRange(
            new CommunicationUnsubscribe { TenantId = x.TenantId, UserId = byAccount.Id, DestinationAddress = "9000000011", Channel = CommunicationChannel.Sms, TokenHash = "a" },
            new CommunicationUnsubscribe { TenantId = x.TenantId, DestinationAddress = "9000000012", Channel = CommunicationChannel.Sms, TokenHash = "b" },
            new CommunicationUnsubscribe { TenantId = x.TenantId, UserId = resubscribed.Id, DestinationAddress = "9000000013", Channel = CommunicationChannel.Sms, TokenHash = "c", IsActive = false },
            new CommunicationUnsubscribe { TenantId = x.TenantId, UserId = otherChannel.Id, DestinationAddress = "other@example.test", Channel = CommunicationChannel.Email, TokenHash = "d" });
        await db.SaveChangesAsync();

        var result = await CampaignAudience.ExcludeOptedOutAsync(db, [byAccount, byAddress, resubscribed, otherChannel], CommunicationChannel.Sms);

        Assert.Equal(new[] { resubscribed.Id, otherChannel.Id }, result.Select(u => u.Id));
    }

    [Fact]
    public async Task DoesNotRequireRecordedConsent()
    {
        var x = Create();
        await using var db = x.Db;
        var noConsent = Person(x.TenantId, "noconsent", "9000000021");

        var result = await CampaignAudience.ExcludeOptedOutAsync(db, [noConsent], CommunicationChannel.WhatsApp);

        Assert.Equal(noConsent.Id, Assert.Single(result).Id);
    }
}
