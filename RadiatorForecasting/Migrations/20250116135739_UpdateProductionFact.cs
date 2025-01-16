using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiatorForecasting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductionFact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionFacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AverageTemperature = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    CompetitorPrice = table.Column<int>(type: "int", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    AluminumRadiatorsProduced = table.Column<int>(type: "int", nullable: false),
                    CopperRadiatorsProduced = table.Column<int>(type: "int", nullable: false),
                    MonthlyForecastAluminum = table.Column<int>(type: "int", nullable: true),
                    MonthlyForecastCopper = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionFacts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionFacts");
        }
    }
}
