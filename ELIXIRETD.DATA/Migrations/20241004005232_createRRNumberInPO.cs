using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createRRNumberInPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RRDate",
                table: "WarehouseReceived",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRNo",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RRDate",
                table: "PoSummaries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RRNo",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RRDate",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "RRNo",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "RRDate",
                table: "PoSummaries");

            migrationBuilder.DropColumn(
                name: "RRNo",
                table: "PoSummaries");
        }
    }
}
