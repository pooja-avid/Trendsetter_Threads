namespace Trendsetter_Threads.Web.Models;

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UserId { get; set;}
}
