using Trendsetter_Threads.API.Data.Entity.DbSet;
using Trendsetter_Threads.API.Data.Entity;
using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trendsetter_Threads.API.DTO.Service;

namespace Trendsetter_Threads.API.Repository.Service;

public class OrderService : IOrderService
{
    private readonly TrendsetterDbContext _db;

    public OrderService(TrendsetterDbContext dbContext) => _db = dbContext;

    public async Task<OperationResult> CreateOrder(OrderModel model)
    {
        try
        {
            var checkProductStock = ValidateOrderQuantities(model.OrderItems);
            if (!checkProductStock.IsSuccess)
            {
                return checkProductStock;
            }

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var orderItems = new List<OrderItem>();
                    var productIds = model.OrderItems.Select(oi => oi.ProductId).ToList();
                    var productsList = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
                    var productsDict = productsList.ToDictionary(p => p.Id);

                    decimal totalPrice = 0.0M;

                    for (int i = 0; i < model.OrderItems.Count; i++)
                    {
                        var orderItem = model.OrderItems[i];
                        var product = productsDict[orderItem.ProductId];
                        product.Stock -= orderItem.Quantity;

                        orderItems.Add(new OrderItem
                        {
                            Price = product.Price,
                            ProductId = orderItem.ProductId,
                            Quantity = orderItem.Quantity,
                        });

                        totalPrice += orderItem.Quantity * product.Price;
                    }

                    var order = new Order
                    {
                        UserId = model.UserId,
                        TotalPrice = totalPrice,
                        Status = model.Status,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _db.Orders.AddAsync(order);
                    await _db.SaveChangesAsync();

                    for (int i = 0; i < orderItems.Count; i++)
                    {
                        orderItems[i].OrderId = order.Id;
                    }

                    _db.Products.UpdateRange(productsList);
                    await _db.OrderItems.AddRangeAsync(orderItems);
                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new OperationResult(true, "Order created successfully.", StatusCodes.Status201Created);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the exception (ex)
                    return new OperationResult(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            return new OperationResult(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<OperationResult<List<OrderModel>>> GetOrders(int userId)
    {
        var operationResult = new OperationResult<List<OrderModel>>();
        try
        {
            var userOrders = await _db.Orders.Where(order => order.UserId == userId).ToListAsync();

            var orderDtoList = new List<OrderModel>();
            for (int i = 0; i < userOrders.Count; i++)
            {
                var currentOrder = userOrders[i];
                orderDtoList.Add(new OrderModel
                {
                    Id = currentOrder.Id,
                    TotalPrice = currentOrder.TotalPrice,
                    UserId = currentOrder.UserId,
                    Status = currentOrder.Status,
                    CreatedAt = currentOrder.CreatedAt
                });
            }

            operationResult.IsSuccess = true;
            operationResult.StatusCode = StatusCodes.Status200OK;
            operationResult.Data = orderDtoList;
            operationResult.Message = "Orders retrieved successfully.";
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            operationResult.IsSuccess = false;
            operationResult.StatusCode = StatusCodes.Status500InternalServerError;
            operationResult.Message = "An error occurred while retrieving orders. Please try again later.";
        }
        return operationResult;
    }

    public async Task<OperationResult<List<OrderItemModel>>> GetOrderDetail(int id)
    {
        var operationResult = new OperationResult<List<OrderItemModel>>();
        try
        {
            var orderItems = await _db.OrderItems.Where(oi => oi.OrderId == id).ToListAsync();
            var productIds = orderItems.Select(oi => oi.ProductId).ToList();
            var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            var productDict = products.ToDictionary(p => p.Id);

            var orderItemDtoList = new List<OrderItemModel>();

            for (int i = 0; i < orderItems.Count; i++)
            {
                var currentOrderItem = orderItems[i];
                var currentProduct = productDict[currentOrderItem.ProductId];

                var orderItemDto = new OrderItemModel
                {
                    Id = currentOrderItem.Id,
                    Price = currentOrderItem.Price,
                    ProductId = currentOrderItem.ProductId,
                    Quantity = currentOrderItem.Quantity,
                    OrderId = currentOrderItem.OrderId,
                    Product = new ProductModel
                    {
                        Id = currentProduct.Id,
                        Name = currentProduct.Name,
                        Description = currentProduct.Description,
                        Price = currentProduct.Price,
                        Stock = currentProduct.Stock
                    }
                };
                orderItemDtoList.Add(orderItemDto);
            }

            operationResult.IsSuccess = true;
            operationResult.StatusCode = StatusCodes.Status200OK;
            operationResult.Message = "Order items retrieved successfully.";
            operationResult.Data = orderItemDtoList;
        }
        catch (Exception ex)
        {
            // Log the exception (ex)
            operationResult.IsSuccess = false;
            operationResult.StatusCode = StatusCodes.Status500InternalServerError;
            operationResult.Message = "An error occurred while retrieving order items. Please try again later.";
        }
        return operationResult;
    }

    private OperationResult ValidateOrderQuantities(List<OrderItemModel> model)
    {
        bool isOutOfStock = false;
        string message = "";

        for (int i = 0; i < model.Count; i++)
        {
            var order = model[i];
            var product = _db.Products.FirstOrDefault(p => p.Id == order.ProductId);

            if (product == null)
            {
                // Handle case where product with given ID does not exist
                continue; // Skip to next order
            }

            int availableStock = product.Stock - order.Quantity;

            if (availableStock < 0)
            {
                isOutOfStock = true;

                if (product.Stock == 0)
                {
                    message += $"{product.Name} is out of stock.";
                }
                else
                {
                    message += $"Only {product.Stock} quantity of {product.Name} is available and ordered quantity is {order.Quantity}.";
                }
            }
        }

        int statusCode = isOutOfStock ? StatusCodes.Status406NotAcceptable : StatusCodes.Status200OK;
        return new OperationResult(!isOutOfStock, message, statusCode);
    }
}
