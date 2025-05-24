using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItemKitchen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KitchenId",
                table: "Item",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_KitchenId",
                table: "Item",
                column: "KitchenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Kitchen_KitchenId",
                table: "Item",
                column: "KitchenId",
                principalTable: "Kitchen",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Kitchen_KitchenId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_KitchenId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "KitchenId",
                table: "Item");
        }
    }
}
