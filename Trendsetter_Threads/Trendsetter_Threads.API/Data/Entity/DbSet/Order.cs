using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Trendsetter_Threads.API.Data.Entity.DbSet;

public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public decimal TotalPrice { get; set; }

    [MaxLength(50)]
    public string Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; }
    
    public ICollection<OrderItem> OrderItems { get; set; }

}
