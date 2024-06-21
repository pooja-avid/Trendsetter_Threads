namespace Trendsetter_Threads.Web.Models;
public class APIResponseResult
{
    public string Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}

public class APIResponseResult<T>
{
    public string Message { get; set; }
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
}

