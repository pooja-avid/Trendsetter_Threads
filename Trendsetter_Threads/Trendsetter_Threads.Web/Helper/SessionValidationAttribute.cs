using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Trendsetter_Threads.Web.Helper;

public class SessionValidationAttribute : ActionFilterAttribute
{
    private readonly string _loginPath = "/";

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Cookies.Keys.Contains("_IndProCookie") && !context.HttpContext.Session.Keys.Contains("UserId"))
        {
            var isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax)
            {
                // Create a JSON object representing the result
                var jsonResult = new
                {
                    statusCode = StatusCodes.Status401Unauthorized,
                    redirectUrl = _loginPath 
                };

                // Serialize the JSON object to a string
                var jsonString = JsonConvert.SerializeObject(jsonResult);

                // Return the JSON string as ContentResult
                context.Result = new ContentResult
                {
                    Content = jsonString,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }
            context.Result = new RedirectResult(_loginPath);
            return;
        }
        base.OnActionExecuting(context);
    }
}
