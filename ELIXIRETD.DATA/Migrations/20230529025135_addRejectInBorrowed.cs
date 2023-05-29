using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addRejectInBorrowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "BorrowedIssues",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsRejectDate",
                table: "BorrowedIssues",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "BorrowedIssueDetails",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsRejectDate",
                table: "BorrowedIssueDetails",
                type: "Date",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsRejectDate",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "BorrowedIssueDetails");

            migrationBuilder.DropColumn(
                name: "IsRejectDate",
                table: "BorrowedIssueDetails");
        }
    }
}
