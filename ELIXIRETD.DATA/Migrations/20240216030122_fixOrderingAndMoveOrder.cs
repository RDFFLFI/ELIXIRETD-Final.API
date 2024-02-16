using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class fixOrderingAndMoveOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Approver",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateApproved",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requestor",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Approver",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateApproved",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HelpdeskNo",
                table: "MoveOrders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Requestor",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approver",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateApproved",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Requestor",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Approver",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "DateApproved",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "HelpdeskNo",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "Requestor",
                table: "MoveOrders");
        }
    }
}
