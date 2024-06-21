using Trendsetter_Threads.Web.Helper;
using Microsoft.AspNetCore.Mvc;

namespace Trendsetter_Threads.Web.Controllers;

[SessionValidation]
public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
