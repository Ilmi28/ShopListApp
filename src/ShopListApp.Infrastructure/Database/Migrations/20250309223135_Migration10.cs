using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopListApp.Migrations;

/// <inheritdoc />
public partial class Migration10 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Products_Categories_CategoryId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_Products_Stores_StoreId",
            table: "Products");

        migrationBuilder.DropForeignKey(
            name: "FK_ShopListProducts_Products_ProductId",
            table: "ShopListProducts");

        migrationBuilder.DropForeignKey(
            name: "FK_ShopListProducts_ShopLists_ShopListId",
            table: "ShopListProducts");

        migrationBuilder.DropForeignKey(
            name: "FK_ShopLists_AspNetUsers_UserId1",
            table: "ShopLists");

        migrationBuilder.DropForeignKey(
            name: "FK_Tokens_AspNetUsers_UserId",
            table: "Tokens");

        migrationBuilder.DropTable(
            name: "ShopListProductLogs");

        migrationBuilder.DropIndex(
            name: "IX_Tokens_UserId",
            table: "Tokens");

        migrationBuilder.DropIndex(
            name: "IX_ShopLists_UserId1",
            table: "ShopLists");

        migrationBuilder.DropIndex(
            name: "IX_ShopListProducts_ProductId",
            table: "ShopListProducts");

        migrationBuilder.DropIndex(
            name: "IX_ShopListProducts_ShopListId",
            table: "ShopListProducts");

        migrationBuilder.DropIndex(
            name: "IX_Products_CategoryId",
            table: "Products");

        migrationBuilder.DropIndex(
            name: "IX_Products_StoreId",
            table: "Products");

        migrationBuilder.DropColumn(
            name: "UserId1",
            table: "ShopLists");

        migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "Tokens",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)");

        migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "ShopLists",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.CreateTable(
            name: "ShopListLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ShopListId = table.Column<int>(type: "int", nullable: false),
                ProductId = table.Column<int>(type: "int", nullable: true),
                Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShopListLogs", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ShopListLogs");

        migrationBuilder.AlterColumn<string>(
            name: "UserId",
            table: "Tokens",
            type: "nvarchar(450)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<int>(
            name: "UserId",
            table: "ShopLists",
            type: "int",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<string>(
            name: "UserId1",
            table: "ShopLists",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "ShopListProductLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProductId = table.Column<int>(type: "int", nullable: false),
                ShopListId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ShopListProductLogs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Tokens_UserId",
            table: "Tokens",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_ShopLists_UserId1",
            table: "ShopLists",
            column: "UserId1");

        migrationBuilder.CreateIndex(
            name: "IX_ShopListProducts_ProductId",
            table: "ShopListProducts",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_ShopListProducts_ShopListId",
            table: "ShopListProducts",
            column: "ShopListId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_CategoryId",
            table: "Products",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_StoreId",
            table: "Products",
            column: "StoreId");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_Categories_CategoryId",
            table: "Products",
            column: "CategoryId",
            principalTable: "Categories",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Products_Stores_StoreId",
            table: "Products",
            column: "StoreId",
            principalTable: "Stores",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ShopListProducts_Products_ProductId",
            table: "ShopListProducts",
            column: "ProductId",
            principalTable: "Products",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ShopListProducts_ShopLists_ShopListId",
            table: "ShopListProducts",
            column: "ShopListId",
            principalTable: "ShopLists",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ShopLists_AspNetUsers_UserId1",
            table: "ShopLists",
            column: "UserId1",
            principalTable: "AspNetUsers",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Tokens_AspNetUsers_UserId",
            table: "Tokens",
            column: "UserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
