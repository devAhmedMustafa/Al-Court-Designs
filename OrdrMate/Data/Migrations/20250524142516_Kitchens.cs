using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Kitchens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kitchen",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kitchen_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KitchenPower",
                columns: table => new
                {
                    BranchId = table.Column<string>(type: "text", nullable: false),
                    KitchenId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Units = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenPower", x => new { x.BranchId, x.KitchenId });
                    table.ForeignKey(
                        name: "FK_KitchenPower_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KitchenPower_Kitchen_KitchenId",
                        column: x => x.KitchenId,
                        principalTable: "Kitchen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_Name_RestaurantId",
                table: "Kitchen",
                columns: new[] { "Name", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kitchen_RestaurantId",
                table: "Kitchen",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchenPower_BranchId_KitchenId",
                table: "KitchenPower",
                columns: new[] { "BranchId", "KitchenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KitchenPower_KitchenId",
                table: "KitchenPower",
                column: "KitchenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KitchenPower");

            migrationBuilder.DropTable(
                name: "Kitchen");
        }
    }
}
