using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroceryTracker.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Stores",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Stores", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "GroceryItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroceryItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_GroceryItems_Categories_CategoryId",
                    column: x => x.CategoryId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GroceryItems_Stores_StoreId",
                    column: x => x.StoreId,
                    principalTable: "Stores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "PriceHistories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GroceryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                OldPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                NewPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PriceHistories", x => x.Id);
                table.ForeignKey(
                    name: "FK_PriceHistories_GroceryItems_GroceryItemId",
                    column: x => x.GroceryItemId,
                    principalTable: "GroceryItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_PriceHistories_Stores_StoreId",
                    column: x => x.StoreId,
                    principalTable: "Stores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_GroceryItems_CategoryId",
            table: "GroceryItems",
            column: "CategoryId");

        migrationBuilder.CreateIndex(
            name: "IX_GroceryItems_StoreId",
            table: "GroceryItems",
            column: "StoreId");

        migrationBuilder.CreateIndex(
            name: "IX_PriceHistories_GroceryItemId",
            table: "PriceHistories",
            column: "GroceryItemId");

        migrationBuilder.CreateIndex(
            name: "IX_PriceHistories_StoreId",
            table: "PriceHistories",
            column: "StoreId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "PriceHistories");

        migrationBuilder.DropTable(
            name: "GroceryItems");

        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "Stores");
    }
}
