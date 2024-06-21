using Trendsetter_Threads.Web.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Trendsetter_Threads.Web.Helper;
public class ApiHelper : IApiHelper
{
    private readonly IConfiguration _configuration;
    public ApiHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<HttpResponseMessage> MakeApiCallAsync(string endPoint, HttpMethod httpMethod, dynamic payload = null)
    {
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        var baseApiUrl = _configuration.GetValue<string>("BaseApiUrl:ApiUrl");

        HttpContent content = null;
        if (payload != null)
        {
            content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
        }
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(baseApiUrl + endPoint, UriKind.Absolute),
            Method = httpMethod,
            Content = content
        };
        
        HttpClient httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(240);
        var httpResponseMessage = await httpClient.SendAsync(httpRequest);
        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> MakeApiCallAsync(string endPoint, HttpMethod httpMethod, HttpContext httpContext, dynamic payload = null)
    {
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };

        var baseApiUrl = _configuration.GetValue<string>("BaseApiUrl:ApiUrl");
        var token = httpContext.Session.GetString("Token");
        HttpContent content = null;
        if (payload != null)
        {
            content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );
        }
        var httpRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(baseApiUrl + endPoint, UriKind.Absolute),
            Method = httpMethod,
            Content = content
        };
        httpRequest.Headers.Add("Authorization", "Bearer " + token);
        HttpClient httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromMinutes(240);
        var httpResponseMessage = await httpClient.SendAsync(httpRequest);
        return httpResponseMessage;
    }
}
