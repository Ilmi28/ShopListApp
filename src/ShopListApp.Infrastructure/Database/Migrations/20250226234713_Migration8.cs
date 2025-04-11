using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopListApp.Migrations;

/// <inheritdoc />
public partial class Migration8 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Categories_Stores_StoreId",
            table: "Categories");

        migrationBuilder.DropIndex(
            name: "IX_Categories_StoreId",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Categories");

        migrationBuilder.DropColumn(
            name: "StoreId",
            table: "Categories");

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "Products",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Products",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Categories",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.InsertData(
            table: "Categories",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { 1, "warzywa" },
                { 2, "owoce" },
                { 3, "pieczywa" },
                { 4, "nabial i jajka" },
                { 5, "mieso" },
                { 6, "dania gotowe" },
                { 7, "napoje" },
                { 8, "mrozone" },
                { 9, "artykuly spozywcze" },
                { 10, "drogeria" },
                { 11, "dla domu" },
                { 12, "dla dzieci" },
                { 13, "dla zwierzat" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Products_Name_StoreId",
            table: "Products",
            columns: new[] { "Name", "StoreId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Categories_Name",
            table: "Categories",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Products_Name_StoreId",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Categories_Name",
            table: "Categories");

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 1);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 5);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 6);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 8);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 9);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 10);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 11);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 12);

        migrationBuilder.DeleteData(
            table: "Categories",
            keyColumn: "Id",
            keyValue: 13);

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "Products",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Products",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Categories",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Categories",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "StoreId",
            table: "Categories",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Categories_StoreId",
            table: "Categories",
            column: "StoreId");

        migrationBuilder.AddForeignKey(
            name: "FK_Categories_Stores_StoreId",
            table: "Categories",
            column: "StoreId",
            principalTable: "Stores",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
