using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addLotNamesInMaterialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotNameId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LotNamesId",
                table: "Materials",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_LotNamesId",
                table: "Materials",
                column: "LotNamesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Lotnames_LotNamesId",
                table: "Materials",
                column: "LotNamesId",
                principalTable: "Lotnames",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Lotnames_LotNamesId",
                table: "Materials");

            migrationBuilder.DropIndex(
                name: "IX_Materials_LotNamesId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LotNameId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "LotNamesId",
                table: "Materials");
        }
    }
}
