using Trendsetter_Threads.API.Data.Models;
using Trendsetter_Threads.API.DTO.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trendsetter_Threads.API.Controllers
{
    [Route("api/")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderDto _order;

        public OrderController(IOrderDto orderDto) => _order = orderDto;

        [HttpPost]
        [Route("orders")]
        public async Task<OperationResult> CreateOrder(OrderModel model) => await _order.CreateOrder(model);

        [HttpGet]
        [Route("orders")]
        public async Task<OperationResult<List<OrderModel>>> GetOrders(int userId) => await _order.GetOrders(userId);

        [HttpGet]
        [Route("orders/{id}")]
        public async Task<OperationResult<List<OrderItemModel>>> GetOrderDetail(int id) => await _order.GetOrderDetail(id);
    }

}
