using Microsoft.EntityFrameworkCore;
using RadiatorForecasting.Models;


namespace RadiatorForecasting.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        // Таблица для фактов выпуска
        public DbSet<ProductionFact> ProductionFacts { get; set; }
        public DbSet<Stock> Stocks { get; set; }

    }
}
