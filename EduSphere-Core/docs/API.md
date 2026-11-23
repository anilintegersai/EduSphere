# EduSphere API v1

All protected endpoints use a bearer token returned by `POST /api/v1/auth/login`. Tenant-scoped calls also require `X-Tenant-ID`. List endpoints introduced by the API platform accept `page`, `pageSize` (maximum 100), `search`, `sortBy`, and `sortDirection`.

Errors use one envelope:

```json
{ "success": false, "data": null, "errors": ["Reason"], "traceId": "request-trace-id" }
```

## Safe retries

Payment creation and bulk import require an `Idempotency-Key` header. A successful response is retained in the database for 24 hours. Reusing the same key and payload replays that response; reusing it with different content returns HTTP 409.

```http
POST /api/v1/finance/payments HTTP/1.1
Authorization: Bearer {token}
X-Tenant-ID: {tenant-guid}
Idempotency-Key: invoice-1042-attempt-1
Content-Type: application/json
```

## Reports and downloads

```http
GET /api/v1/reports?page=1&pageSize=25&search=attendance
POST /api/v1/reports/{definitionId}/execute?format=Excel
GET /api/v1/reports/executions/{executionId}/download
```

## Settings, integrations, and bulk master data

```http
GET /api/v1/settings/tenant?page=1&pageSize=25
PUT /api/v1/settings/tenant/{key}
GET /api/v1/settings/integrations?search=smtp
GET /api/v1/bulk/lookups/export
POST /api/v1/bulk/lookups/preview
POST /api/v1/bulk/lookups/import
```

Bulk endpoints accept `multipart/form-data` with a `file` field. Preview validates every row without writing data. Import refuses files containing any invalid row.

## Applicant API

Public form discovery is restricted to 20 requests per minute per client IP. Applicant application endpoints require the dedicated Applicant authentication cookie and always constrain records by both applicant account and tenant.

```http
GET /api/v1/applicant/forms?tenantId={tenant-guid}&branchId={branch-guid}
GET /api/v1/applicant/applications
GET /api/v1/applicant/applications/{applicationId}
```

The authenticated API is limited to 120 requests per minute per caller/IP. JSON request bodies are limited to 25 MB and multipart uploads to 20 MB unless an endpoint has a stricter limit.
