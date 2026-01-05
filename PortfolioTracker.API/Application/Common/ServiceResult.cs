namespace Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data,
        StatusCode = 200
    };

    public static ServiceResult<T> Fail(string errorMessage, int statusCode = 400) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        StatusCode = statusCode
    };

    public static ServiceResult<T> Conflict(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage,
        StatusCode = 409
    };
}

public static class ServiceResult
{
    public static ServiceResult<T> Ok<T>(T data) => ServiceResult<T>.Ok(data);
    public static ServiceResult<T> Fail<T>(string errorMessage, int statusCode = 400) => ServiceResult<T>.Fail(errorMessage, statusCode);
    public static ServiceResult<T> Conflict<T>(string errorMessage) => ServiceResult<T>.Conflict(errorMessage);
}

