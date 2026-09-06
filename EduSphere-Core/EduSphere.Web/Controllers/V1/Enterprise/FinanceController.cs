using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.Enterprise;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Common;
using EduSphere.Domain.Entities;
using EduSphere.Web.Authorization;
using EduSphere.Web.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.Enterprise;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/finance")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.FinanceManager)]
public class FinanceController : EnterpriseControllerBase
{
    private readonly ICrudService<FeeStructure> _structures;
    private readonly ICrudService<FeeComponent> _components;
    private readonly ICrudService<DiscountRule> _discounts;
    private readonly ICrudService<Scholarship> _scholarships;
    private readonly ICrudService<StudentFeeAssignment> _assignments;
    private readonly ICrudService<FeeInvoice> _invoices;
    private readonly ICrudService<FeeInvoiceLine> _invoiceLines;
    private readonly ICrudService<FeePayment> _payments;
    private readonly ICrudService<FeeReceipt> _receipts;
    private readonly ICrudService<Branch> _branches;
    private readonly ICrudService<StudentProfile> _students;

    public FinanceController(
        ICrudService<FeeStructure> structures,
        ICrudService<FeeComponent> components,
        ICrudService<DiscountRule> discounts,
        ICrudService<Scholarship> scholarships,
        ICrudService<StudentFeeAssignment> assignments,
        ICrudService<FeeInvoice> invoices,
        ICrudService<FeeInvoiceLine> invoiceLines,
        ICrudService<FeePayment> payments,
        ICrudService<FeeReceipt> receipts,
        ICrudService<Branch> branches,
        ICrudService<StudentProfile> students,
        IBranchAccessService branchAccess)
        : base(branchAccess)
    {
        _structures = structures;
        _components = components;
        _discounts = discounts;
        _scholarships = scholarships;
        _assignments = assignments;
        _invoices = invoices;
        _invoiceLines = invoiceLines;
        _payments = payments;
        _receipts = receipts;
        _branches = branches;
        _students = students;
    }

    [HttpGet("fee-structures")]
    public async Task<IActionResult> GetFeeStructures([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeeStructureDto>>.Ok((await FilterByBranchAsync(_structures, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("fee-structures")]
    public async Task<IActionResult> CreateFeeStructure([FromBody] CreateFeeStructureRequest request)
    {
        if (await ValidateBranchRequestAsync(request.BranchId) is { } invalid) return invalid;
        var created = await _structures.CreateAsync(Apply(new FeeStructure(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeeStructureDto>.Ok(created.Map()));
    }

    [HttpPut("fee-structures/{id:guid}")]
    public async Task<IActionResult> UpdateFeeStructure(Guid id, [FromBody] UpdateFeeStructureRequest request)
    {
        if (await ExistingBranchScopedAsync(_structures, id, x => x.BranchId, "Fee structure") is not { } existing) return NotFoundResult<FeeStructure>(id, "Fee structure");
        if (!await CanUseBranchAsync(existing.BranchId) || !await CanUseBranchAsync(request.BranchId)) return Forbid();
        await _structures.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("fee-structures/{id:guid}")]
    public Task<IActionResult> DeleteFeeStructure(Guid id)
        => DeleteBranchScopedAsync(_structures, id, x => x.BranchId, "Fee structure");

    [HttpGet("fee-components")]
    public async Task<IActionResult> GetFeeComponents([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeeComponentDto>>.Ok((await FilterByBranchAsync(_components, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("fee-components")]
    public async Task<IActionResult> CreateFeeComponent([FromBody] CreateFeeComponentRequest request)
    {
        if (await ValidateBranchRequestAsync(request.BranchId) is { } invalid) return invalid;
        if (await _structures.GetAsync(request.FeeStructureId) is not { } structure || structure.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected fee structure was not found for the branch."));
        var created = await _components.CreateAsync(Apply(new FeeComponent(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeeComponentDto>.Ok(created.Map()));
    }

    [HttpPut("fee-components/{id:guid}")]
    public async Task<IActionResult> UpdateFeeComponent(Guid id, [FromBody] UpdateFeeComponentRequest request)
    {
        if (await ExistingBranchScopedAsync(_components, id, x => x.BranchId, "Fee component") is null) return NotFoundResult<FeeComponent>(id, "Fee component");
        if (await ValidateBranchRequestAsync(request.BranchId) is { } invalid) return invalid;
        if (await _structures.GetAsync(request.FeeStructureId) is not { } structure || structure.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected fee structure was not found for the branch."));
        await _components.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("fee-components/{id:guid}")]
    public Task<IActionResult> DeleteFeeComponent(Guid id)
        => DeleteBranchScopedAsync(_components, id, x => x.BranchId, "Fee component");

    [HttpGet("discount-rules")]
    public async Task<IActionResult> GetDiscountRules([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<DiscountRuleDto>>.Ok((await FilterByBranchAsync(_discounts, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("discount-rules")]
    public async Task<IActionResult> CreateDiscountRule([FromBody] CreateDiscountRuleRequest request)
    {
        if (!await CanUseOptionalBranchAsync(request.BranchId)) return Forbid();
        var created = await _discounts.CreateAsync(Apply(new DiscountRule(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<DiscountRuleDto>.Ok(created.Map()));
    }

    [HttpPut("discount-rules/{id:guid}")]
    public async Task<IActionResult> UpdateDiscountRule(Guid id, [FromBody] UpdateDiscountRuleRequest request)
    {
        if (await ExistingBranchScopedAsync(_discounts, id, x => x.BranchId, "Discount rule") is null) return NotFoundResult<DiscountRule>(id, "Discount rule");
        if (!await CanUseOptionalBranchAsync(request.BranchId)) return Forbid();
        await _discounts.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("discount-rules/{id:guid}")]
    public Task<IActionResult> DeleteDiscountRule(Guid id)
        => DeleteBranchScopedAsync(_discounts, id, x => x.BranchId, "Discount rule");

    [HttpGet("scholarships")]
    public async Task<IActionResult> GetScholarships([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<ScholarshipDto>>.Ok((await FilterByBranchAsync(_scholarships, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("scholarships")]
    public async Task<IActionResult> CreateScholarship([FromBody] CreateScholarshipRequest request)
    {
        if (await ValidateStudentBranchAsync(request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        var created = await _scholarships.CreateAsync(Apply(new Scholarship(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ScholarshipDto>.Ok(created.Map()));
    }

    [HttpPut("scholarships/{id:guid}")]
    public async Task<IActionResult> UpdateScholarship(Guid id, [FromBody] UpdateScholarshipRequest request)
    {
        if (await ExistingBranchScopedAsync(_scholarships, id, x => x.BranchId, "Scholarship") is null) return NotFoundResult<Scholarship>(id, "Scholarship");
        if (await ValidateStudentBranchAsync(request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        await _scholarships.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("scholarships/{id:guid}")]
    public Task<IActionResult> DeleteScholarship(Guid id)
        => DeleteBranchScopedAsync(_scholarships, id, x => x.BranchId, "Scholarship");

    [HttpGet("student-fee-assignments")]
    public async Task<IActionResult> GetStudentFeeAssignments([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<StudentFeeAssignmentDto>>.Ok((await FilterByBranchAsync(_assignments, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("student-fee-assignments")]
    public async Task<IActionResult> CreateStudentFeeAssignment([FromBody] CreateStudentFeeAssignmentRequest request)
    {
        if (await ValidateStudentBranchAsync(request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        if (await _structures.GetAsync(request.FeeStructureId) is not { } structure || structure.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected fee structure was not found for the branch."));
        var created = await _assignments.CreateAsync(Apply(new StudentFeeAssignment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<StudentFeeAssignmentDto>.Ok(created.Map()));
    }

    [HttpPut("student-fee-assignments/{id:guid}")]
    public async Task<IActionResult> UpdateStudentFeeAssignment(Guid id, [FromBody] UpdateStudentFeeAssignmentRequest request)
    {
        if (await ExistingBranchScopedAsync(_assignments, id, x => x.BranchId, "Student fee assignment") is null) return NotFoundResult<StudentFeeAssignment>(id, "Student fee assignment");
        if (await ValidateStudentBranchAsync(request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        if (await _structures.GetAsync(request.FeeStructureId) is not { } structure || structure.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected fee structure was not found for the branch."));
        await _assignments.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("student-fee-assignments/{id:guid}")]
    public Task<IActionResult> DeleteStudentFeeAssignment(Guid id)
        => DeleteBranchScopedAsync(_assignments, id, x => x.BranchId, "Student fee assignment");

    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeeInvoiceDto>>.Ok((await FilterByBranchAsync(_invoices, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateFeeInvoiceRequest request)
    {
        if (await ValidateInvoiceAsync(request.StudentFeeAssignmentId, request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        var created = await _invoices.CreateAsync(Apply(new FeeInvoice(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeeInvoiceDto>.Ok(created.Map()));
    }

    [HttpPut("invoices/{id:guid}")]
    public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateFeeInvoiceRequest request)
    {
        if (await ExistingBranchScopedAsync(_invoices, id, x => x.BranchId, "Invoice") is null) return NotFoundResult<FeeInvoice>(id, "Invoice");
        if (await ValidateInvoiceAsync(request.StudentFeeAssignmentId, request.StudentProfileId, request.BranchId) is { } invalid) return invalid;
        await _invoices.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("invoices/{id:guid}")]
    public Task<IActionResult> DeleteInvoice(Guid id)
        => DeleteBranchScopedAsync(_invoices, id, x => x.BranchId, "Invoice");

    [HttpGet("invoice-lines")]
    public async Task<IActionResult> GetInvoiceLines([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeeInvoiceLineDto>>.Ok((await FilterByBranchAsync(_invoiceLines, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("invoice-lines")]
    public async Task<IActionResult> CreateInvoiceLine([FromBody] CreateFeeInvoiceLineRequest request)
    {
        if (await ValidateInvoiceChildAsync(request.FeeInvoiceId, request.BranchId) is { } invalid) return invalid;
        var created = await _invoiceLines.CreateAsync(Apply(new FeeInvoiceLine(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeeInvoiceLineDto>.Ok(created.Map()));
    }

    [HttpPut("invoice-lines/{id:guid}")]
    public async Task<IActionResult> UpdateInvoiceLine(Guid id, [FromBody] UpdateFeeInvoiceLineRequest request)
    {
        if (await ExistingBranchScopedAsync(_invoiceLines, id, x => x.BranchId, "Invoice line") is null) return NotFoundResult<FeeInvoiceLine>(id, "Invoice line");
        if (await ValidateInvoiceChildAsync(request.FeeInvoiceId, request.BranchId) is { } invalid) return invalid;
        await _invoiceLines.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("invoice-lines/{id:guid}")]
    public Task<IActionResult> DeleteInvoiceLine(Guid id)
        => DeleteBranchScopedAsync(_invoiceLines, id, x => x.BranchId, "Invoice line");

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeePaymentDto>>.Ok((await FilterByBranchAsync(_payments, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("payments")]
    public async Task<IActionResult> CreatePayment([FromBody] CreateFeePaymentRequest request)
    {
        if (await ValidateInvoiceChildAsync(request.FeeInvoiceId, request.BranchId) is { } invalid) return invalid;
        var created = await _payments.CreateAsync(Apply(new FeePayment(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeePaymentDto>.Ok(created.Map()));
    }

    [HttpPut("payments/{id:guid}")]
    public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] UpdateFeePaymentRequest request)
    {
        if (await ExistingBranchScopedAsync(_payments, id, x => x.BranchId, "Payment") is null) return NotFoundResult<FeePayment>(id, "Payment");
        if (await ValidateInvoiceChildAsync(request.FeeInvoiceId, request.BranchId) is { } invalid) return invalid;
        await _payments.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("payments/{id:guid}")]
    public Task<IActionResult> DeletePayment(Guid id)
        => DeleteBranchScopedAsync(_payments, id, x => x.BranchId, "Payment");

    [HttpGet("receipts")]
    public async Task<IActionResult> GetReceipts([FromQuery] Guid? branchId)
        => Ok(ApiResponse<IEnumerable<FeeReceiptDto>>.Ok((await FilterByBranchAsync(_receipts, x => x.BranchId, branchId)).Select(x => x.Map())));

    [HttpPost("receipts")]
    public async Task<IActionResult> CreateReceipt([FromBody] CreateFeeReceiptRequest request)
    {
        if (await _payments.GetAsync(request.FeePaymentId) is not { } payment || payment.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected payment was not found for the branch."));
        if (!await CanUseBranchAsync(request.BranchId)) return Forbid();
        var created = await _receipts.CreateAsync(Apply(new FeeReceipt(), request));
        return StatusCode(StatusCodes.Status201Created, ApiResponse<FeeReceiptDto>.Ok(created.Map()));
    }

    [HttpPut("receipts/{id:guid}")]
    public async Task<IActionResult> UpdateReceipt(Guid id, [FromBody] UpdateFeeReceiptRequest request)
    {
        if (await ExistingBranchScopedAsync(_receipts, id, x => x.BranchId, "Receipt") is null) return NotFoundResult<FeeReceipt>(id, "Receipt");
        if (await _payments.GetAsync(request.FeePaymentId) is not { } payment || payment.BranchId != request.BranchId)
            return BadRequest(ApiResponse<object>.Fail("Selected payment was not found for the branch."));
        if (!await CanUseBranchAsync(request.BranchId)) return Forbid();
        await _receipts.UpdateAsync(id, e => Apply(e, request));
        return NoContent();
    }

    [HttpDelete("receipts/{id:guid}")]
    public Task<IActionResult> DeleteReceipt(Guid id)
        => DeleteBranchScopedAsync(_receipts, id, x => x.BranchId, "Receipt");

    private async Task<IActionResult?> ValidateBranchRequestAsync(Guid branchId)
    {
        if (await _branches.GetAsync(branchId) is null)
            return BadRequest(ApiResponse<object>.Fail("Selected branch was not found in this tenant."));
        return await CanUseBranchAsync(branchId) ? null : Forbid();
    }

    private async Task<IActionResult?> ValidateStudentBranchAsync(Guid studentId, Guid branchId)
    {
        if (await ValidateBranchRequestAsync(branchId) is { } invalid) return invalid;
        if (await _students.GetAsync(studentId) is not { } student || student.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected student was not found for the branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateInvoiceAsync(Guid assignmentId, Guid studentId, Guid branchId)
    {
        if (await ValidateStudentBranchAsync(studentId, branchId) is { } invalid) return invalid;
        if (await _assignments.GetAsync(assignmentId) is not { } assignment || assignment.BranchId != branchId || assignment.StudentProfileId != studentId)
            return BadRequest(ApiResponse<object>.Fail("Selected fee assignment was not found for the student and branch."));
        return null;
    }

    private async Task<IActionResult?> ValidateInvoiceChildAsync(Guid invoiceId, Guid branchId)
    {
        if (await ValidateBranchRequestAsync(branchId) is { } invalid) return invalid;
        if (await _invoices.GetAsync(invoiceId) is not { } invoice || invoice.BranchId != branchId)
            return BadRequest(ApiResponse<object>.Fail("Selected invoice was not found for the branch."));
        return null;
    }

    private async Task<T?> ExistingBranchScopedAsync<T>(ICrudService<T> service, Guid id, Func<T, Guid?> branchSelector, string displayName)
        where T : class, IGuidEntity, ISoftDeletable
    {
        var entity = await service.GetAsync(id);
        return entity is not null && await CanUseOptionalBranchAsync(branchSelector(entity)) ? entity : null;
    }

    private NotFoundObjectResult NotFoundResult<T>(Guid id, string displayName)
        => NotFound(ApiResponse<object>.Fail($"{displayName} {id} was not found."));

    private static FeeStructure Apply(FeeStructure entity, CreateFeeStructureRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.AcademicYearId = request.AcademicYearId;
        entity.CourseId = request.CourseId;
        entity.BatchId = request.BatchId;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Frequency = request.Frequency;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.IsActive = request.IsActive;
        entity.Notes = request.Notes;
        return entity;
    }

    private static FeeComponent Apply(FeeComponent entity, CreateFeeComponentRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.FeeStructureId = request.FeeStructureId;
        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.Amount = request.Amount;
        entity.IsOptional = request.IsOptional;
        entity.DueDaysFromStart = request.DueDaysFromStart;
        entity.SortOrder = request.SortOrder;
        entity.LedgerCode = request.LedgerCode;
        entity.Notes = request.Notes;
        return entity;
    }

    private static DiscountRule Apply(DiscountRule entity, CreateDiscountRuleRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.CourseId = request.CourseId;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Type = request.Type;
        entity.Value = request.Value;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.IsActive = request.IsActive;
        entity.EligibilityCriteria = request.EligibilityCriteria;
        return entity;
    }

    private static Scholarship Apply(Scholarship entity, CreateScholarshipRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Type = request.Type;
        entity.Value = request.Value;
        entity.AwardedOn = request.AwardedOn;
        entity.ValidUntil = request.ValidUntil;
        entity.IsActive = request.IsActive;
        entity.SponsorName = request.SponsorName;
        entity.Notes = request.Notes;
        return entity;
    }

    private static StudentFeeAssignment Apply(StudentFeeAssignment entity, CreateStudentFeeAssignmentRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.FeeStructureId = request.FeeStructureId;
        entity.DiscountRuleId = request.DiscountRuleId;
        entity.AssignedOn = request.AssignedOn;
        entity.Status = request.Status;
        entity.CustomDiscountAmount = request.CustomDiscountAmount;
        entity.Notes = request.Notes;
        return entity;
    }

    private static FeeInvoice Apply(FeeInvoice entity, CreateFeeInvoiceRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.StudentFeeAssignmentId = request.StudentFeeAssignmentId;
        entity.StudentProfileId = request.StudentProfileId;
        entity.InvoiceNumber = request.InvoiceNumber;
        entity.InvoiceDate = request.InvoiceDate;
        entity.DueDate = request.DueDate;
        entity.Status = request.Status;
        entity.SubTotal = request.SubTotal;
        entity.DiscountAmount = request.DiscountAmount;
        entity.FineAmount = request.FineAmount;
        entity.TotalAmount = request.TotalAmount;
        entity.PaidAmount = request.PaidAmount;
        entity.Notes = request.Notes;
        return entity;
    }

    private static FeeInvoiceLine Apply(FeeInvoiceLine entity, CreateFeeInvoiceLineRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.FeeInvoiceId = request.FeeInvoiceId;
        entity.FeeComponentId = request.FeeComponentId;
        entity.Description = request.Description;
        entity.ComponentType = request.ComponentType;
        entity.Amount = request.Amount;
        entity.SortOrder = request.SortOrder;
        return entity;
    }

    private static FeePayment Apply(FeePayment entity, CreateFeePaymentRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.FeeInvoiceId = request.FeeInvoiceId;
        entity.PaymentNumber = request.PaymentNumber;
        entity.PaidOn = request.PaidOn;
        entity.Amount = request.Amount;
        entity.Mode = request.Mode;
        entity.Status = request.Status;
        entity.TransactionReference = request.TransactionReference;
        entity.ReceivedByUserId = request.ReceivedByUserId;
        entity.Notes = request.Notes;
        return entity;
    }

    private static FeeReceipt Apply(FeeReceipt entity, CreateFeeReceiptRequest request)
    {
        entity.BranchId = request.BranchId;
        entity.FeePaymentId = request.FeePaymentId;
        entity.ReceiptNumber = request.ReceiptNumber;
        entity.IssuedOn = request.IssuedOn;
        entity.PdfStoragePath = request.PdfStoragePath;
        entity.IssuedByUserId = request.IssuedByUserId;
        entity.Notes = request.Notes;
        return entity;
    }
}
