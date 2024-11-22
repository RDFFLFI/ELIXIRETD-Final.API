using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateFuelRegisterDriver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Driver",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "FuelRegisters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_UserId",
                table: "FuelRegisters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Users_UserId",
                table: "FuelRegisters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Users_UserId",
                table: "FuelRegisters");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_UserId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FuelRegisters");

            migrationBuilder.AddColumn<string>(
                name: "Driver",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
