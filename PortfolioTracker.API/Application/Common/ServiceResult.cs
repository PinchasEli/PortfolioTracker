namespace Application.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ServiceResult<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ServiceResult<T> Fail(string errorMessage) => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}

public static class ServiceResult
{
    public static ServiceResult<T> Ok<T>(T data) => ServiceResult<T>.Ok(data);
    public static ServiceResult<T> Fail<T>(string errorMessage) => ServiceResult<T>.Fail(errorMessage);
}

