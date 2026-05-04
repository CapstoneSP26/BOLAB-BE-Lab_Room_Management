namespace BookLAB.Application.Common.Models;

/// <summary>
/// Generic API response wrapper for all endpoints
/// </summary>
public class ApiResponse<T>
{
    public T Data { get; set; } = default!;
    public string? Message { get; set; }
    public List<ErrorDetail>? Errors { get; set; }

    public ApiResponse() { }

    public ApiResponse(T data)
    {
        Data = data;
    }

    public ApiResponse(T data, string message)
    {
        Data = data;
        Message = message;
    }
}

/// <summary>
/// Error detail for validation errors
/// </summary>
public class ErrorDetail
{
    public string Field { get; set; } = default!;
    public string Message { get; set; } = default!;
}
