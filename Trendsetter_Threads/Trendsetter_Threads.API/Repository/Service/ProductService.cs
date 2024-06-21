using Trendsetter_Threads.API.Data.Entity;
using Trendsetter_Threads.API.Data.Entity.DbSet;
using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Trendsetter_Threads.API.Repository.Service;

public class ProductService : IProductService
{
    private readonly TrendsetterDbContext _db;

    public ProductService(TrendsetterDbContext dbContext) => _db = dbContext;

    public async Task<OperationResult<List<ProductModel>>> GetProducts()
    {
        var operationResult = new OperationResult<List<ProductModel>>();
        try
        {
            var products = await _db.Products
                                    .Where(product => !product.IsDeleted)
                                    .ToListAsync();

            var productModels = new List<ProductModel>();
            for (int i = 0; i < products.Count; i++)
            {
                var currentProduct = products[i];
                productModels.Add(new ProductModel
                {
                    Id = currentProduct.Id,
                    Name = currentProduct.Name,
                    Stock = currentProduct.Stock,
                    Description = currentProduct.Description,
                    Price = currentProduct.Price,
                    CreatedAt = currentProduct.CreatedAt
                });
            }

            operationResult.IsSuccess = true;
            operationResult.StatusCode = StatusCodes.Status200OK;
            operationResult.Message = "Active products retrieved successfully.";
            operationResult.Data = productModels;
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            operationResult.IsSuccess = false;
            operationResult.StatusCode = StatusCodes.Status500InternalServerError;
            operationResult.Message = "An error occurred while retrieving active products. Please try again later.";
        }
        return operationResult;
    }

    public async Task<OperationResult> CreateProduct(ProductModel model)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

            if (user == null)
            {
                return new OperationResult(false, "User not found. Invalid user details.", StatusCodes.Status404NotFound);
            }

            if (!user.IsAdmin)
            {
                return new OperationResult(false, "Unauthorized: You don't have rights to create a product.", StatusCodes.Status403Forbidden);
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CreatedAt = model.CreatedAt
            };

            await _db.Products.AddAsync(product);
            await _db.SaveChangesAsync();

            return new OperationResult(true, "Product created successfully.", StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return new OperationResult(false, "Failed to create product. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }
    public async Task<OperationResult> UpdateProduct(ProductModel model, int id)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

            if (user == null)
            {
                return new OperationResult(false, "User not found. Invalid user details.", StatusCodes.Status404NotFound);
            }

            if (!user.IsAdmin)
            {
                return new OperationResult(false, "Unauthorized: You don't have rights to update a product.", StatusCodes.Status403Forbidden);
            }

            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new OperationResult(false, "Product not found.", StatusCodes.Status404NotFound);
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;

            _db.Update(product);
            await _db.SaveChangesAsync();

            return new OperationResult(true, "Product updated successfully.", StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return new OperationResult(false, "Failed to update product. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<OperationResult> DeleteProduct(int id, int userId)
    {
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return new OperationResult(false, "User not found. Invalid user details.", StatusCodes.Status404NotFound);
            }

            if (!user.IsAdmin)
            {
                return new OperationResult(false, "Unauthorized: You don't have rights to delete a product.", StatusCodes.Status403Forbidden);
            }

            var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new OperationResult(false, "Product not found.", StatusCodes.Status404NotFound);
            }

            product.IsDeleted = true;
            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return new OperationResult(true, "Product deleted successfully.", StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return new OperationResult(false, "Failed to delete product. Please try again later.", StatusCodes.Status500InternalServerError);
        }
    }

}
