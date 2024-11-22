using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateLotSectionInMaterials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Lotnames_LotNamesId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "LotNamesId",
                table: "Materials",
                newName: "LotSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_LotNamesId",
                table: "Materials",
                newName: "IX_Materials_LotSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_LotSections_LotSectionId",
                table: "Materials",
                column: "LotSectionId",
                principalTable: "LotSections",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_LotSections_LotSectionId",
                table: "Materials");

            migrationBuilder.RenameColumn(
                name: "LotSectionId",
                table: "Materials",
                newName: "LotNamesId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_LotSectionId",
                table: "Materials",
                newName: "IX_Materials_LotNamesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Lotnames_LotNamesId",
                table: "Materials",
                column: "LotNamesId",
                principalTable: "Lotnames",
                principalColumn: "Id");
        }
    }
}
