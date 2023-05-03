using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddSubcategoryIdInMaterials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_ItemCategories_ItemCategoryId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "ItemCategoryId",
                table: "Materials",
                newName: "SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_ItemCategoryId",
                table: "Materials",
                newName: "IX_Materials_SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_SubCategories_SubCategoryId",
                table: "Materials",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_SubCategories_SubCategoryId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "Materials",
                newName: "ItemCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_SubCategoryId",
                table: "Materials",
                newName: "IX_Materials_ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_ItemCategories_ItemCategoryId",
                table: "Materials",
                column: "ItemCategoryId",
                principalTable: "ItemCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
