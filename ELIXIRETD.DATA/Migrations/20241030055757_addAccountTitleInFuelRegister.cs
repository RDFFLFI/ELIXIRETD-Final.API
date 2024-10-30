using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountTitleInFuelRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Account_Title_Code",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Title_Name",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account_Title_Code",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Account_Title_Name",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "FuelRegisters");
        }
    }
}
