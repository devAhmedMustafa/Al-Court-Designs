using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class PreperationTimeForItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<decimal>(
                name: "PreperationTime",
                table: "Item",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreperationTime",
                table: "Item");

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
    }
}
