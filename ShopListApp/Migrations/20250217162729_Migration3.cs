using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopListApp.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email_IsDeleted",
                table: "AspNetUsers",
                columns: new[] { "Email", "IsDeleted" },
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserName_IsDeleted",
                table: "AspNetUsers",
                columns: new[] { "UserName", "IsDeleted" },
                unique: true,
                filter: "[UserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserName_IsDeleted",
                table: "AspNetUsers");
        }
    }
}
