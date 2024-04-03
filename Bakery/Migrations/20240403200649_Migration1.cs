using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBGList_Chap7.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Allergen",
                columns: table => new
                {
                    AllergenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllergenName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergen", x => x.AllergenId);
                });

            migrationBuilder.CreateTable(
                name: "StockAllergen",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false),
                    AllergenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAllergen", x => new { x.StockId, x.AllergenId });
                    table.ForeignKey(
                        name: "FK_StockAllergen_Allergen_AllergenId",
                        column: x => x.AllergenId,
                        principalTable: "Allergen",
                        principalColumn: "AllergenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAllergen_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockAllergen_AllergenId",
                table: "StockAllergen",
                column: "AllergenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockAllergen");

            migrationBuilder.DropTable(
                name: "Allergen");
        }
    }
}
