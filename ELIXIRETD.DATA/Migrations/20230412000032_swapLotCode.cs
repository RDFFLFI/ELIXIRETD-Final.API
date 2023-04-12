using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class swapLotCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotNameCode",
                table: "LotNames");

            migrationBuilder.AddColumn<string>(
                name: "LotCode",
                table: "LotCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LotCode",
                table: "LotCategories");

            migrationBuilder.AddColumn<string>(
                name: "LotNameCode",
                table: "LotNames",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
