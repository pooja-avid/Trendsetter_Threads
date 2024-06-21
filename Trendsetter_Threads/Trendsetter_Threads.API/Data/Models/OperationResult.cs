namespace Trendsetter_Threads.API.Data.Models;
public class OperationResult
{
    public bool IsSuccess { get; }

    public string Message { get; }

    public int StatusCode { get; set; } = StatusCodes.Status200OK;

    public OperationResult(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public OperationResult(bool isSuccess, string message, int statusCode)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
    }

    public static OperationResult ReturnSuccess()
    {
        return new OperationResult(true);
    }

    public static OperationResult ReturnFailed(string message)
    {
        return new OperationResult(false, message, StatusCodes.Status400BadRequest);
    }

    public static OperationResult ReturnNotFound(string message)
    {
        return new OperationResult(false, message, StatusCodes.Status404NotFound);
    }
}


public class OperationResult<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
}
