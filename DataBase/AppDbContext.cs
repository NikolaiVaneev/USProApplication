using Microsoft.EntityFrameworkCore;
using USProApplication.DataBase.Entities;

namespace USProApplication.DataBase
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Counterparty> Counterparties { get; set; }
    }
}
