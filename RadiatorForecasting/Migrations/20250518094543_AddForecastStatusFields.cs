using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiatorForecasting.Migrations
{
    /// <inheritdoc />
    public partial class AddForecastStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ForecastStatus",
                table: "ProductionFacts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ProductionFacts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForecastStatus",
                table: "ProductionFacts");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "ProductionFacts");
        }
    }
}
