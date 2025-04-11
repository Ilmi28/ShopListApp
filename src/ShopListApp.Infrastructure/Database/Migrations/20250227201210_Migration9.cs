using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopListApp.Migrations;

/// <inheritdoc />
public partial class Migration9 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Stores",
            columns: new[] { "Id", "IsDeleted", "Name" },
            values: new object[] { 1, false, "Biedronka" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Stores",
            keyColumn: "Id",
            keyValue: 1);
    }
}
