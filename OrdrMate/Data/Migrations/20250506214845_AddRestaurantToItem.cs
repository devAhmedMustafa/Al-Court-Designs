using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestaurantId",
                table: "Item",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Item");
        }
    }
}
