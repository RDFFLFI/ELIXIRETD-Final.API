using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAssetCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Asset_Code",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Asset_Name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plate_No",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Asset_Code",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Asset_Name",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plate_No",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asset_Code",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Asset_Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Plate_No",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Asset_Code",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "Asset_Name",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "Plate_No",
                table: "MoveOrders");
        }
    }
}
