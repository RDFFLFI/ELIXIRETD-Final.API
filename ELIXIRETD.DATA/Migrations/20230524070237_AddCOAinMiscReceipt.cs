using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddCOAinMiscReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "MiscellaneousReceipts");
        }
    }
}
