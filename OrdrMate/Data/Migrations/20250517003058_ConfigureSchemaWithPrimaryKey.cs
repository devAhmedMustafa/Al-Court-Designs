using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureSchemaWithPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_ManagerId",
                table: "Restaurant",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_Name_CategoryName_RestaurantId",
                table: "Item",
                columns: new[] { "Name", "CategoryName", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_RestaurantId",
                table: "Item",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_RestaurantId",
                table: "Category",
                column: "RestaurantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Restaurant_RestaurantId",
                table: "Category",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Restaurant_RestaurantId",
                table: "Item",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Manager_ManagerId",
                table: "Restaurant",
                column: "ManagerId",
                principalTable: "Manager",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Restaurant_RestaurantId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_Restaurant_RestaurantId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Manager_ManagerId",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_ManagerId",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Item_Name_CategoryName_RestaurantId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_RestaurantId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Category_RestaurantId",
                table: "Category");
        }
    }
}
