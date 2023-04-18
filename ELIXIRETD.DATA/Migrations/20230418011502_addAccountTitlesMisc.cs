using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addAccountTitlesMisc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "CompanyCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "MiscellaneousIssues");

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
        }
    }
}
