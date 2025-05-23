using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class TableOfRestaurantBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Table",
                columns: table => new
                {
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    BranchId = table.Column<string>(type: "text", nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Table", x => new { x.TableNumber, x.BranchId });
                    table.ForeignKey(
                        name: "FK_Table_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Table_BranchId",
                table: "Table",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_TableNumber_BranchId",
                table: "Table",
                columns: new[] { "TableNumber", "BranchId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Table");
        }
    }
}
