using EduSphere.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers;
[ApiController,AllowAnonymous,Route("api/finance/webhooks")]
public sealed class FinanceWebhooksController(IFinanceAccountingService finance):ControllerBase
{
    [HttpPost("{tenantId:guid}/{provider}")]
    public async Task<IActionResult> Receive(Guid tenantId,string provider){using var reader=new StreamReader(Request.Body);var body=await reader.ReadToEndAsync();var signature=provider.Equals("stripe",StringComparison.OrdinalIgnoreCase)?Request.Headers["Stripe-Signature"].ToString():Request.Headers["X-Razorpay-Signature"].ToString();return await finance.ProcessWebhookAsync(tenantId,provider,signature,body,HttpContext.RequestAborted)?Ok():Unauthorized();}
}
