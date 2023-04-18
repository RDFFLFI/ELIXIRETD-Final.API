using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountTitlesBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "BorrowedIssueDetails");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "BorrowedIssues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "BorrowedIssues");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "BorrowedIssueDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
