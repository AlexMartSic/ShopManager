using Microsoft.EntityFrameworkCore;
using ShopManager.Domain.Entities;

namespace ShopManager.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure the Product entity

            ////One customer has many orders
            //modelBuilder.Entity<Customer>()
            //    .HasMany(e => e.CurrentOrders)
            //    .WithOne(e => e.Customer)
            //    .HasForeignKey(e => e.CustomerId);

            ////One order has many order lines
            //modelBuilder.Entity<Order>()
            //    .HasMany(e => e.Lines)
            //    .WithOne(e => e.Order)
            //    .HasForeignKey(e => e.OrderId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Code)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<OrderLine>()
                .HasIndex(ol => new { ol.OrderId, ol.LineNumber })
                .IsUnique();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
    }
}
