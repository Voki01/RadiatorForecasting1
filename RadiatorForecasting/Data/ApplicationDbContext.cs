using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RadiatorForecasting.Models;

namespace RadiatorForecasting.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Таблицы приложения
        public DbSet<ProductionFact> ProductionFacts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ReleasedProduction> ReleasedProductions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
    }
}

