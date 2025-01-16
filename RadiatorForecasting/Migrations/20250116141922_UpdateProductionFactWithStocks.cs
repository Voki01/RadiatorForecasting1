using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiatorForecasting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductionFactWithStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentAluminumStock",
                table: "ProductionFacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentCopperStock",
                table: "ProductionFacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedAluminumStock",
                table: "ProductionFacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecommendedCopperStock",
                table: "ProductionFacts",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAluminumStock",
                table: "ProductionFacts");

            migrationBuilder.DropColumn(
                name: "CurrentCopperStock",
                table: "ProductionFacts");

            migrationBuilder.DropColumn(
                name: "RecommendedAluminumStock",
                table: "ProductionFacts");

            migrationBuilder.DropColumn(
                name: "RecommendedCopperStock",
                table: "ProductionFacts");
        }
    }
}
