using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createRRNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RRDate",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRNo",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRDate",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRNo",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RRDate",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "RRNo",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "RRDate",
                table: "PoSummaries");

            migrationBuilder.DropColumn(
                name: "RRNo",
                table: "PoSummaries");
        }
    }
}
