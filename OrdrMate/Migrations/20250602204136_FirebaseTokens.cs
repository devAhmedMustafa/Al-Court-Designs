using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrdrMate.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FirebaseToken",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirebaseToken", x => x.Token);
                    table.ForeignKey(
                        name: "FK_FirebaseToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FirebaseToken_UserId",
                table: "FirebaseToken",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirebaseToken");
        }
    }
}
