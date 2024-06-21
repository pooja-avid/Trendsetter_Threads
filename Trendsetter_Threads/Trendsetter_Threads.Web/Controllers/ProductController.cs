using Trendsetter_Threads.Web.Helper;
using Trendsetter_Threads.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Trendsetter_Threads.Web.Controllers;

[SessionValidation]
public class ProductController : Controller
{
    private readonly IApiHelper _apiHelper;
    public ProductController(IApiHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<APIResponseResult<List<ProductModel>>> GetProductsList()
    {
        var responseMessage = await _apiHelper.MakeApiCallAsync("products", HttpMethod.Get, HttpContext, null);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<List<ProductModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<string>> CreateProduct([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.MakeApiCallAsync("products", HttpMethod.Post, HttpContext, model);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<string>> EditProduct([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.MakeApiCallAsync("products/"+ model.Id, HttpMethod.Put, HttpContext, model);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<APIResponseResult<string>> DeleteProductById([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.MakeApiCallAsync("products/"+ model.Id + "?userId="+ model.UserId, HttpMethod.Delete, HttpContext, null);
        var responseAsync = await CommonMethod.HandleApiResponseAsync<string>(responseMessage);
        return responseAsync;
    }
}
