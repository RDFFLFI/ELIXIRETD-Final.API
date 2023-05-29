using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addApprovedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproveBy",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedReturnedBy",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectBy",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproveBy",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "ApprovedReturnedBy",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "RejectBy",
                table: "BorrowedIssues");
        }
    }
}
