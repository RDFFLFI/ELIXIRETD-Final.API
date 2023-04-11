using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "MiscellaneousIssueDetail");

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
        }
    }
}
