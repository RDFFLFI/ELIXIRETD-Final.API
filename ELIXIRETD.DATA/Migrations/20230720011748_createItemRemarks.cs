using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createItemRemarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "TransactOrder",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "MoveOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemRemarks",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "TransactOrder");

            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "MoveOrders");

            migrationBuilder.DropColumn(
                name: "ItemRemarks",
                table: "BorrowedIssueDetails");
        }
    }
}
