using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Branches_And_Requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branch",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Lantitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<string>(type: "text", nullable: false),
                    BranchManagerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branch_Manager_BranchManagerId",
                        column: x => x.BranchManagerId,
                        principalTable: "Manager",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Branch_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Lantitude = table.Column<float>(type: "real", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchRequest_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branch_BranchManagerId",
                table: "Branch",
                column: "BranchManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Lantitude_Longitude_RestaurantId",
                table: "Branch",
                columns: new[] { "Lantitude", "Longitude", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_Phone",
                table: "Branch",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branch_RestaurantId",
                table: "Branch",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchRequest_Lantitude_Longitude_RestaurantId",
                table: "BranchRequest",
                columns: new[] { "Lantitude", "Longitude", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchRequest_RestaurantId",
                table: "BranchRequest",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "BranchRequest");
        }
    }
}
