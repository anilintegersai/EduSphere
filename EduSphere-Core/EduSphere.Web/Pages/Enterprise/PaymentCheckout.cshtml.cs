using System.Text.Json;
using System.Security.Claims;
using EduSphere.Domain.Entities;
using EduSphere.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EduSphere.Web.Pages.Enterprise;
[Authorize]
public sealed class PaymentCheckoutModel(TenantDbContext db,EduSphere.Web.Security.IBranchAccessService branchAccess):PageModel
{
 public OnlinePaymentTransaction Transaction{get;private set;}=null!;public string KeyId{get;private set;}="";public string InstitutionName{get;private set;}="EduSphere";public string KeyJson=>JsonSerializer.Serialize(KeyId);public string NameJson=>JsonSerializer.Serialize(InstitutionName);public string OrderJson=>JsonSerializer.Serialize(Transaction.GatewayOrderId);public string CurrencyJson=>JsonSerializer.Serialize(Transaction.Currency);
 public async Task<IActionResult> OnGetAsync(string order){var tx=await db.OnlinePaymentTransactions.Include(x=>x.FeeInvoice).SingleOrDefaultAsync(x=>x.GatewayOrderId==order);if(tx?.FeeInvoice is null)return NotFound();var uid=Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier),out var parsed)?parsed:Guid.Empty;var studentOwns=tx.FeeInvoice.StudentProfileId.HasValue&&await db.StudentProfiles.AnyAsync(x=>x.Id==tx.FeeInvoice.StudentProfileId&&x.UserId==uid);var parentOwns=tx.FeeInvoice.StudentProfileId.HasValue&&await db.StudentGuardians.AnyAsync(x=>x.StudentProfileId==tx.FeeInvoice.StudentProfileId&&x.ParentUserId==uid&&x.HasPortalAccess);if(!studentOwns&&!parentOwns&&!await branchAccess.CanAccessBranchAsync(User,tx.BranchId))return Forbid();var setting=await db.TenantIntegrationSettings.AsNoTracking().SingleOrDefaultAsync(x=>x.IntegrationKey==tx.GatewayProviderKey&&x.IsEnabled);if(setting is null)return NotFound();using var config=JsonDocument.Parse(setting.ConfigurationJson);KeyId=config.RootElement.TryGetProperty("keyId",out var key)?key.GetString()??"":"";InstitutionName=config.RootElement.TryGetProperty("businessName",out var name)?name.GetString()??InstitutionName:InstitutionName;Transaction=tx;return Page();}
}
