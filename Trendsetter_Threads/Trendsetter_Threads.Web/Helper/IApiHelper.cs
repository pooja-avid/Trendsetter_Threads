namespace Trendsetter_Threads.Web.Helper;
public interface IApiHelper
{
    public Task<HttpResponseMessage> MakeApiCallAsync(string endPoint, HttpMethod httpMethod, dynamic payload = null);
    public Task<HttpResponseMessage> MakeApiCallAsync(string endPoint, HttpMethod httpMethod, HttpContext httpContext, dynamic payload = null);
}
