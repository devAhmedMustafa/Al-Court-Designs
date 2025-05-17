using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeCategoryForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Item",
                newName: "CategoryName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                columns: new[] { "Name", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryName_RestaurantId",
                table: "Item",
                columns: new[] { "CategoryName", "RestaurantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Category_CategoryName_RestaurantId",
                table: "Item",
                columns: new[] { "CategoryName", "RestaurantId" },
                principalTable: "Category",
                principalColumns: new[] { "Name", "RestaurantId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Category_CategoryName_RestaurantId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_CategoryName_RestaurantId",
                table: "Item");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "Item",
                newName: "Category");
        }
    }
}
