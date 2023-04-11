using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class removeReturnedborrowedDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consume",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "IsReturned",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "ReturnedDate",
                table: "BorrowedIssues");

            migrationBuilder.DropColumn(
                name: "ReturnedQuantity",
                table: "BorrowedIssues");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Consume",
                table: "BorrowedIssues",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsReturned",
                table: "BorrowedIssues",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedDate",
                table: "BorrowedIssues",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnedQuantity",
                table: "BorrowedIssues",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
