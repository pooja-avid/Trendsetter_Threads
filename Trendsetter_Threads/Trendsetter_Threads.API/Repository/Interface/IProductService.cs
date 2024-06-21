﻿using Trendsetter_Threads.API.Data.Models;

namespace Trendsetter_Threads.API.Repository.Interface;

public interface IProductService
{
    public Task<OperationResult<List<ProductModel>>> GetProducts();
    public Task<OperationResult> CreateProduct(ProductModel model);
    public Task<OperationResult> UpdateProduct(ProductModel model, int id);
    public Task<OperationResult> DeleteProduct(int id, int userId);
}