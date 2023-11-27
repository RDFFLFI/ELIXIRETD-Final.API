using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateMaterialByItemCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemCategoryId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_ItemCategoryId",
                table: "Materials",
                column: "ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_ItemCategories_ItemCategoryId",
                table: "Materials",
                column: "ItemCategoryId",
                principalTable: "ItemCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_ItemCategories_ItemCategoryId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_ItemCategoryId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ItemCategoryId",
                table: "Materials");
        }
    }
}
