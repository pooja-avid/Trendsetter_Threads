using Trendsetter_Threads.Web.Helper;
using Trendsetter_Threads.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Trendsetter_Threads.Web.Controllers;
public class UserController : Controller
{
    private readonly IApiHelper _apiHelper;
    public UserController(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult signup()
    {
        return View();
    }

    [HttpPost]
    public async Task<APIResponseResult<UserModel>> Login([FromBody] LoginModel model)
    {
        var responseMessage = await _apiHelper.MakeApiCallAsync("User/Login", HttpMethod.Post, model);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<UserModel>(responseMessage);
        if (responseAsync.IsSuccess)
        {
            HttpContext.Session.SetString("Token", responseAsync.Data.Token);
            HttpContext.Session.SetString("Username", responseAsync.Data.Username);
            HttpContext.Session.SetInt32("UserId", responseAsync.Data.Id);
            HttpContext.Session.SetInt32("IsAdmin", responseAsync.Data.IsAdmin ? 1 : 0);
        }
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<string>> Register([FromBody] RegisterModel model)
    {
        var responseMessage = await _apiHelper.MakeApiCallAsync("User/Register", HttpMethod.Post, model);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpGet]
    public async Task<string> Logout()
    {

        HttpContext.Session.Remove("Token");
        HttpContext.Session.Remove("Username");
        HttpContext.Session.Remove("IsAdmin");
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Clear();
        var expiredCookie = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(-1),
            IsEssential = true,
            HttpOnly = true,
        };
        Response.Cookies.Append("_IndProCookie", "", expiredCookie);

        return "Logout succcessfully.";
    }
}
