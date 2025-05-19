using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class RestaurantToManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ManagerId1",
                table: "Restaurant",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_ManagerId1",
                table: "Restaurant",
                column: "ManagerId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_Manager_ManagerId1",
                table: "Restaurant",
                column: "ManagerId1",
                principalTable: "Manager",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_Manager_ManagerId1",
                table: "Restaurant");

            migrationBuilder.DropIndex(
                name: "IX_Restaurant_ManagerId1",
                table: "Restaurant");

            migrationBuilder.DropColumn(
                name: "ManagerId1",
                table: "Restaurant");
        }
    }
}
