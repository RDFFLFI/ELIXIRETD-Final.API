using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addApprovedInBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "BorrowedIssues",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsApprovedDate",
                table: "BorrowedIssues",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedReturned",
                table: "BorrowedIssues",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsApprovedReturnedDate",
                table: "BorrowedIssues",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "BorrowedIssueDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsApprovedDate",
                table: "BorrowedIssueDetails",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedReturned",
                table: "BorrowedIssueDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsApprovedReturnedDate",
                table: "BorrowedIssueDetails",
                type: "Date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsApprovedDate",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsApprovedReturned",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsApprovedReturnedDate",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "IsApprovedDate",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "IsApprovedReturned",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "IsApprovedReturnedDate",
                table: "BorrowedIssueDetails");
        }
    }
}
