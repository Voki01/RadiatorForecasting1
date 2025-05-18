using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiatorForecasting.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionDateToProductionFacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectionDate",
                table: "ProductionFacts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionDate",
                table: "ProductionFacts");
        }
    }
}
