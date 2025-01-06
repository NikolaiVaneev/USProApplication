using Microsoft.EntityFrameworkCore;
using USProApplication.DataBase.Entities;

namespace USProApplication.DataBase
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Executor)
                      .WithMany()
                      .HasForeignKey(o => o.ExecutorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Customer)
                      .WithMany()
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithMany(s => s.Orders)
                .UsingEntity<Dictionary<string, object>>(
                "OrderService", 
                j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId"),
                j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId"),
                j =>
                {
                    j.HasKey("OrderId", "ServiceId"); 
                    j.ToTable("OrderServices"); 
                });
        }

        public DbSet<Service> Services { get; set; }
        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<Order> Orders { get; set; } 
    }
}
