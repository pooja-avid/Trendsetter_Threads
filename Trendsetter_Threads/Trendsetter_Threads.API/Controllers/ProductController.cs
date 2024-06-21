using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.DTO.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trendsetter_Threads.API.Controllers;

[Route("api/")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductDto _product;
    public ProductController(IProductDto productDto) => _product = productDto;

    [HttpGet]
    [Route("products")]
    public async Task<OperationResult<List<ProductModel>>> GetProducts() => await _product.GetProducts();


    [HttpPost]
    [Route("products")]
    public async Task<OperationResult> CreateProduct(ProductModel model) => await _product.CreateProduct(model);


    [HttpPut]
    [Route("products/{id}")]
    public async Task<OperationResult> UpdateProduct(ProductModel model, int id) => await _product.UpdateProduct(model, id);


    [HttpDelete]
    [Route("products/{id}")]
    public async Task<OperationResult> DeleteProduct(int id, int userId) => await _product.DeleteProduct(id, userId);
}
