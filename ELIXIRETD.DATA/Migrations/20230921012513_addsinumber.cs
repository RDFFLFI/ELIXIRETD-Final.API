using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addsinumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SINumber",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SINumber",
                table: "WarehouseReceived");
        }
    }
}
