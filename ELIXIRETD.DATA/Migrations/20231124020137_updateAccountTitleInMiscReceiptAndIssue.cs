using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateAccountTitleInMiscReceiptAndIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MiscellaneousReceipts");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "MiscellaneousIssues");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MiscellaneousIssues");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "EmpId",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MiscellaneousIssueDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "AccountCode",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "AccountTitles",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "MiscellaneousIssueDetail");

            migrationBuilder.AddColumn<string>(
                name: "AccountCode",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountTitles",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MiscellaneousReceipts",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "EmpId",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "MiscellaneousIssues",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
