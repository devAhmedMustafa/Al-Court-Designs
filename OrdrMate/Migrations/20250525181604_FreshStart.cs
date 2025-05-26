using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Migrations
{
    /// <inheritdoc />
    public partial class FreshStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    ManagerId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurant_User_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_Branch_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Branch_User_BranchManagerId",
                        column: x => x.BranchManagerId,
                        principalTable: "User",
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

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => new { x.Name, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_Category_Restaurant_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    PreperationTime = table.Column<decimal>(type: "numeric", nullable: false),
                    KitchenId = table.Column<string>(type: "text", nullable: true),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Category_CategoryName_RestaurantId",
                        columns: x => new { x.CategoryName, x.RestaurantId },
                        principalTable: "Category",
                        principalColumns: new[] { "Name", "RestaurantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Item_Kitchen_KitchenId",
                        column: x => x.KitchenId,
                        principalTable: "Kitchen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Item_Restaurant_RestaurantId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name_RestaurantId",
                table: "Category",
                columns: new[] { "Name", "RestaurantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_RestaurantId",
                table: "Category",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryName_RestaurantId",
                table: "Item",
                columns: new[] { "CategoryName", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_Item_KitchenId",
                table: "Item",
                column: "KitchenId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_ManagerId",
                table: "Restaurant",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurant_Name",
                table: "Restaurant",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Table_BranchId",
                table: "Table",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Table_TableNumber_BranchId",
                table: "Table",
                columns: new[] { "TableNumber", "BranchId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchRequest");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "KitchenPower");

            migrationBuilder.DropTable(
                name: "Table");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Kitchen");

            migrationBuilder.DropTable(
                name: "Branch");

            migrationBuilder.DropTable(
                name: "Restaurant");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
