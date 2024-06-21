using Trendsetter_Threads.API.Data.Entity.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Trendsetter_Threads.API.Data.Entity;
public class TrendsetterDbContext: DbContext
{
    public TrendsetterDbContext(DbContextOptions<TrendsetterDbContext> options)
          : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

