using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addReportNumberAndEmpIdInBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportNumber",
                table: "BorrowedConsumes",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "ReportNumber",
                table: "BorrowedConsumes");
        }
    }
}
