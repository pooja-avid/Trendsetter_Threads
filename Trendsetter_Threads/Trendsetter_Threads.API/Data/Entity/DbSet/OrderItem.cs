using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trendsetter_Threads.API.Data.Entity.DbSet;
public class OrderItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey("Order")]
    public int OrderId { get; set; }
    
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }

    public ICollection<Order> Orders { get; set; }
    
    public ICollection<Product> Products { get; set; }
}
