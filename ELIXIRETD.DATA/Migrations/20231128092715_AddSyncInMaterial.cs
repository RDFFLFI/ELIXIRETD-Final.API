using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddSyncInMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Material_No",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifyBy",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Materials",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusSync",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "Materials",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ItemCategory_No",
                table: "ItemCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModifyBy",
                table: "ItemCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "ItemCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusSync",
                table: "ItemCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncDate",
                table: "ItemCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Material_No",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ModifyBy",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "StatusSync",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ItemCategory_No",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "ModifyBy",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "StatusSync",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "SyncDate",
                table: "ItemCategories");
        }
    }
}
