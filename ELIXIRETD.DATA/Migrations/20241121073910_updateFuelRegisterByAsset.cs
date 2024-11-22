using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateFuelRegisterByAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asset",
                table: "FuelRegisterDetails");

            migrationBuilder.DropColumn(
                name: "Odometer",
                table: "FuelRegisterDetails");

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Odometer",
                table: "FuelRegisters",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Asset",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Odometer",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "FuelRegisterDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Odometer",
                table: "FuelRegisterDetails",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
