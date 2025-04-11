using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopListApp.Migrations;

/// <inheritdoc />
public partial class Migration4 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_Email_IsDeleted",
            table: "AspNetUsers");

        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_UserName_IsDeleted",
            table: "AspNetUsers");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_Email_NormalizedEmail_IsDeleted",
            table: "AspNetUsers",
            columns: new[] { "Email", "NormalizedEmail", "IsDeleted" },
            unique: true,
            filter: "[Email] IS NOT NULL AND [NormalizedEmail] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_UserName_NormalizedUserName_IsDeleted",
            table: "AspNetUsers",
            columns: new[] { "UserName", "NormalizedUserName", "IsDeleted" },
            unique: true,
            filter: "[UserName] IS NOT NULL AND [NormalizedUserName] IS NOT NULL");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_Email_NormalizedEmail_IsDeleted",
            table: "AspNetUsers");

        migrationBuilder.DropIndex(
            name: "IX_AspNetUsers_UserName_NormalizedUserName_IsDeleted",
            table: "AspNetUsers");

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
}
