using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class deleteReturnQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnQuantity",
                table: "BorrowedIssueDetails");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "BorrowedIssueDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnQuantity",
                table: "BorrowedIssueDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
