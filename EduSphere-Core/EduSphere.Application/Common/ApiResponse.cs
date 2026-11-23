namespace EduSphere.Application.Common;

/// <summary>
/// Consistent response envelope for all API endpoints.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IEnumerable<string>? Errors { get; init; }
    public string? TraceId { get; init; }
    public object? Meta { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error, string? traceId = null)
        => new() { Success = false, Errors = new[] { error }, TraceId = traceId };

    public static ApiResponse<T> Fail(IEnumerable<string> errors, string? traceId = null)
        => new() { Success = false, Errors = errors, TraceId = traceId };
}
