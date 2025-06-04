using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Migrations
{
    /// <inheritdoc />
    public partial class TakeawayAndIndoorPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indoor",
                columns: table => new
                {
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    BranchId = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indoor", x => new { x.TableNumber, x.BranchId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_Indoor_Branch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Indoor_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Takeaway",
                columns: table => new
                {
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Takeaway", x => new { x.OrderId, x.OrderNumber });
                    table.ForeignKey(
                        name: "FK_Takeaway_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Indoor_BranchId",
                table: "Indoor",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Indoor_OrderId",
                table: "Indoor",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indoor");

            migrationBuilder.DropTable(
                name: "Takeaway");
        }
    }
}
