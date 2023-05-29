using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addApprovedByInDetailsBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproveBy",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedReturnedBy",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectBy",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveBy",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "ApprovedReturnedBy",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "RejectBy",
                table: "BorrowedIssueDetails");
        }
    }
}
