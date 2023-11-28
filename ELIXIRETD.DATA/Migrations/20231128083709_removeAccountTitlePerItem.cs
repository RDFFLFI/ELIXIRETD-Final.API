using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class removeAccountTitlePerItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials");

            migrationBuilder.DropTable(
                name: "AccountTitles");

            migrationBuilder.DropIndex(
                name: "IX_Materials_AccountTitleId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "AccountPName",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "AccountTitleId",
                table: "Materials");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountPName",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountTitleId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountTitles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCategoryId = table.Column<int>(type: "int", nullable: false),
                    AccountPName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountTitles_ItemCategories_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalTable: "ItemCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_AccountTitleId",
                table: "Materials",
                column: "AccountTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTitles_ItemCategoryId",
                table: "AccountTitles",
                column: "ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials",
                column: "AccountTitleId",
                principalTable: "AccountTitles",
                principalColumn: "Id");
        }
    }
}
