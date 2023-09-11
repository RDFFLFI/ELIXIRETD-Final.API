using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class Deletesubcategorymodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_SubCategories_SubCategoryId",
                table: "Materials");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.RenameColumn(
                name: "SubCategoryName",
                table: "Materials",
                newName: "AccountPName");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Materials",
                newName: "AccountTitleId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_SubCategoryId",
                table: "Materials",
                newName: "IX_Materials_AccountTitleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials",
                column: "AccountTitleId",
                principalTable: "AccountTitles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "AccountTitleId",
                table: "Materials",
                newName: "SubCategoryId");

            migrationBuilder.RenameColumn(
                name: "AccountPName",
                table: "Materials",
                newName: "SubCategoryName");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_AccountTitleId",
                table: "Materials",
                newName: "IX_Materials_SubCategoryId");

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCategoryId = table.Column<int>(type: "int", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SubCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategories_ItemCategories_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalTable: "ItemCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategories_ItemCategoryId",
                table: "SubCategories",
                column: "ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_SubCategories_SubCategoryId",
                table: "Materials",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
