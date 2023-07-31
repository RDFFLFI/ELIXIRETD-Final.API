using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createUnitCostInBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "EmpId",
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

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "BorrowedIssueDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "EmpId",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "BorrowedIssueDetails");
        }
    }
}
