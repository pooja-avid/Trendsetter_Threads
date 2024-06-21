using Trendsetter_Threads.API.Data.Models;

namespace Trendsetter_Threads.API.DTO.Interface;

public interface IOrderDto
{
    public Task<OperationResult> CreateOrder(OrderModel model);
    public Task<OperationResult<List<OrderModel>>> GetOrders(int userId);
    public Task<OperationResult<List<OrderItemModel>>> GetOrderDetail(int id);
}
