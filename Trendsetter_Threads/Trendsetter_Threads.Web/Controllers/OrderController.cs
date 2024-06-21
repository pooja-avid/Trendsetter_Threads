using Trendsetter_Threads.Web.Helper;
using Trendsetter_Threads.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Trendsetter_Threads.Web.Controllers;

[SessionValidation]
public class OrderController : Controller
{
    private readonly IApiHelper _apiHelper;
    public OrderController(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<APIResponseResult<List<OrderModel>>> GetOrdersDetails()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.MakeApiCallAsync("orders?userId=" + userId, HttpMethod.Get, HttpContext, null);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<List<OrderModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<List<OrderItemModel>>> GetOrderDetails([FromBody] OrderModel model)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.MakeApiCallAsync("orders/" + model.Id, HttpMethod.Get, HttpContext, null);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<List<OrderItemModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<string>> CreateOrder([FromBody] OrderModel model)
    {
        model.Status = "Pending";
        model.UserId = HttpContext.Session.GetInt32("UserId").Value;
        var responseMessage = await _apiHelper.MakeApiCallAsync("orders", HttpMethod.Post, HttpContext, model);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<string>(responseMessage);
        return responseAsync;
    }
}