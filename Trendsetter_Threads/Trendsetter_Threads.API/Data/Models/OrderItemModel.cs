namespace Trendsetter_Threads.API.Data.Models;

public class OrderItemModel
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public ProductModel? Product { get; set; }
}
