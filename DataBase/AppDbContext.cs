using Microsoft.EntityFrameworkCore;
using USProApplication.DataBase.Entities;

namespace USProApplication.DataBase
{
    public class AppDbContext : DbContext
    {
        public DbSet<Service> Services { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = AppConfiguration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString);
        }
    }
}
