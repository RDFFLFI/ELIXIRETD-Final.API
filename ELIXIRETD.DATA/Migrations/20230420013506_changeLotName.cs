using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class changeLotName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotNames_LotCategories_LotCategoryId",
                table: "LotNames");

            migrationBuilder.DropTable(
                name: "LotCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LotNames",
                table: "LotNames");

            migrationBuilder.DropIndex(
                name: "IX_LotNames_LotCategoryId",
                table: "LotNames");

            migrationBuilder.DropColumn(
                name: "LotCategoryId",
                table: "LotNames");

            migrationBuilder.RenameTable(
                name: "LotNames",
                newName: "Lotnames");

            migrationBuilder.RenameColumn(
                name: "SectionName",
                table: "Lotnames",
                newName: "LotName");

            migrationBuilder.AddColumn<string>(
                name: "LotCode",
                table: "Lotnames",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lotnames",
                table: "Lotnames",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LotSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotNamesId = table.Column<int>(type: "int", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotSections_Lotnames_LotNamesId",
                        column: x => x.LotNamesId,
                        principalTable: "Lotnames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotSections_LotNamesId",
                table: "LotSections",
                column: "LotNamesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotSections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lotnames",
                table: "Lotnames");

            migrationBuilder.DropColumn(
                name: "LotCode",
                table: "Lotnames");

            migrationBuilder.RenameTable(
                name: "Lotnames",
                newName: "LotNames");

            migrationBuilder.RenameColumn(
                name: "LotName",
                table: "LotNames",
                newName: "SectionName");

            migrationBuilder.AddColumn<int>(
                name: "LotCategoryId",
                table: "LotNames",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LotNames",
                table: "LotNames",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LotCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LotCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotNames_LotCategoryId",
                table: "LotNames",
                column: "LotCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LotNames_LotCategories_LotCategoryId",
                table: "LotNames",
                column: "LotCategoryId",
                principalTable: "LotCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
