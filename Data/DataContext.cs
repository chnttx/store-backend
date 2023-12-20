using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data;

public class DataContext : DbContext
{

    public DataContext()
    {
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<PaymentDetail> PaymentDetails { get; set; }
    
    #region Required
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerID);
        modelBuilder.Entity<Customer>()
            .HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId);
        modelBuilder.Entity<Order>()
            .HasKey(o => o.OrderId);
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Payments)
            .WithOne(pd => pd.Order)
            .HasForeignKey(pd => pd.OrderId);
        modelBuilder.Entity<PaymentDetail>()
            .HasKey(p => p.PaymentId);
        modelBuilder.Entity<Shop>()
            .HasKey(s => s.ShopId);
        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Items)
            .WithOne(i => i.Shop)
            .HasForeignKey(i => i.ShopId);
        modelBuilder.Entity<Item>()
            .HasKey(i => i.ItemId);
        modelBuilder.Entity<OrderItem>()
            .HasKey(oi => new { oi.OrderId, oi.ItemId });
    }
    #endregion
}