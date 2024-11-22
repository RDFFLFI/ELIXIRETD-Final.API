using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addRejectinFuelRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is_Reject",
                table: "FuelRegisters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Reject_By",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reject_Remarks",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Reject",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Reject_By",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Reject_Remarks",
                table: "FuelRegisters");
        }
    }
}
