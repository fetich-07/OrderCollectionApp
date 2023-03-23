using Microsoft.EntityFrameworkCore;
using OrderCollectionApp.Models;

namespace OrderCollectionApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Provider> Providers { get; set; }
    }
}
