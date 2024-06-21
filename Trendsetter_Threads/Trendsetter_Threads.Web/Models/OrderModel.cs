namespace Trendsetter_Threads.Web.Models;

public class OrderModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemModel> OrderItems { get; set; }
}
