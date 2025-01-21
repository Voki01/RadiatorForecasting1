using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiatorForecasting.Migrations
{
    /// <inheritdoc />
    public partial class AddReleasedProductions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReleasedProductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductionFactId = table.Column<int>(type: "int", nullable: false),
                    AluminumQuantity = table.Column<int>(type: "int", nullable: false),
                    CopperQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleasedProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleasedProductions_ProductionFacts_ProductionFactId",
                        column: x => x.ProductionFactId,
                        principalTable: "ProductionFacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReleasedProductions_ProductionFactId",
                table: "ReleasedProductions",
                column: "ProductionFactId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReleasedProductions");
        }
    }
}
