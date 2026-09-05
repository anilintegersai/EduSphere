using Asp.Versioning;
using EduSphere.Application.Common;
using EduSphere.Application.DTOs.People;
using EduSphere.Application.Interfaces;
using EduSphere.Domain.Entities;
using EduSphere.Domain.Enums;
using EduSphere.Web.Authorization;
using EduSphere.Web.Controllers;
using EduSphere.Web.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduSphere.Web.Controllers.V1.People;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/profile-documents")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = AuthorizationPolicies.TenantAdmin)]
[RequireTenant]
public class ProfileDocumentsController : ApiControllerBase
{
    private readonly ICrudService<ProfileDocument> _documents;
    private readonly ICrudService<StudentProfile> _students;
    private readonly ICrudService<TeacherProfile> _teachers;

    public ProfileDocumentsController(
        ICrudService<ProfileDocument> documents,
        ICrudService<StudentProfile> students,
        ICrudService<TeacherProfile> teachers)
    {
        _documents = documents;
        _students = students;
        _teachers = teachers;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProfileDocumentOwnerType? ownerType, [FromQuery] Guid? ownerId)
    {
        var items = await _documents.ListAsync(d =>
            (!ownerType.HasValue || d.OwnerType == ownerType.Value) &&
            (!ownerId.HasValue || d.OwnerId == ownerId.Value));
        return Ok(ApiResponse<IEnumerable<ProfileDocumentDto>>.Ok(items.Select(Map)));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _documents.GetAsync(id);
        return entity is null
            ? NotFound(ApiResponse<ProfileDocumentDto>.Fail($"Profile document {id} was not found."))
            : Ok(ApiResponse<ProfileDocumentDto>.Ok(Map(entity)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProfileDocumentRequest request)
    {
        var validationError = await ValidateOwnerAsync(request.OwnerType, request.OwnerId);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var created = await _documents.CreateAsync(new ProfileDocument
        {
            OwnerType = request.OwnerType,
            OwnerId = request.OwnerId,
            DocumentType = request.DocumentType,
            DisplayName = request.DisplayName,
            FileName = request.FileName,
            ContentType = request.ContentType,
            StoragePath = request.StoragePath,
            SizeBytes = request.SizeBytes,
            Notes = request.Notes
        });
        return StatusCode(StatusCodes.Status201Created, ApiResponse<ProfileDocumentDto>.Ok(Map(created)));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProfileDocumentRequest request)
    {
        var validationError = await ValidateOwnerAsync(request.OwnerType, request.OwnerId);
        if (validationError is not null)
            return BadRequest(ApiResponse<object>.Fail(validationError));

        var updated = await _documents.UpdateAsync(id, e =>
        {
            e.OwnerType = request.OwnerType;
            e.OwnerId = request.OwnerId;
            e.DocumentType = request.DocumentType;
            e.DisplayName = request.DisplayName;
            e.FileName = request.FileName;
            e.ContentType = request.ContentType;
            e.StoragePath = request.StoragePath;
            e.SizeBytes = request.SizeBytes;
            e.Notes = request.Notes;
            e.IsVerified = request.IsVerified;
            e.VerifiedBy = request.VerifiedBy;
            e.VerifiedOn = request.VerifiedOn;
        });
        return updated ? NoContent() : NotFound(ApiResponse<object>.Fail($"Profile document {id} was not found."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
        => await _documents.SoftDeleteAsync(id)
            ? NoContent()
            : NotFound(ApiResponse<object>.Fail($"Profile document {id} was not found."));

    private async Task<string?> ValidateOwnerAsync(ProfileDocumentOwnerType ownerType, Guid ownerId)
    {
        return ownerType switch
        {
            ProfileDocumentOwnerType.Student when await _students.GetAsync(ownerId) is null =>
                $"Student {ownerId} was not found in this tenant.",
            ProfileDocumentOwnerType.Teacher when await _teachers.GetAsync(ownerId) is null =>
                $"Teacher {ownerId} was not found in this tenant.",
            _ => null
        };
    }

    private static ProfileDocumentDto Map(ProfileDocument e) => new ProfileDocumentDto
    {
        OwnerType = e.OwnerType,
        OwnerId = e.OwnerId,
        DocumentType = e.DocumentType,
        DisplayName = e.DisplayName,
        FileName = e.FileName,
        ContentType = e.ContentType,
        StoragePath = e.StoragePath,
        SizeBytes = e.SizeBytes,
        UploadedOn = e.UploadedOn,
        IsVerified = e.IsVerified,
        VerifiedBy = e.VerifiedBy,
        VerifiedOn = e.VerifiedOn,
        Notes = e.Notes
    }.WithMetadata(e);
}
