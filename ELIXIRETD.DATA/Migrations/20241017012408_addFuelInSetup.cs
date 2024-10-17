using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addFuelInSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters");

            migrationBuilder.RenameColumn(
                name: "MaterialId",
                table: "FuelRegisters",
                newName: "FuelId");

            migrationBuilder.RenameIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters",
                newName: "IX_FuelRegisters_FuelId");

            migrationBuilder.CreateTable(
                name: "Fuels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Added_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fuels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fuels_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fuels_MaterialId",
                table: "Fuels",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Fuels_FuelId",
                table: "FuelRegisters",
                column: "FuelId",
                principalTable: "Fuels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Fuels_FuelId",
                table: "FuelRegisters");

            migrationBuilder.DropTable(
                name: "Fuels");

            migrationBuilder.RenameColumn(
                name: "FuelId",
                table: "FuelRegisters",
                newName: "MaterialId");

            migrationBuilder.RenameIndex(
                name: "IX_FuelRegisters_FuelId",
                table: "FuelRegisters",
                newName: "IX_FuelRegisters_MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id");
        }
    }
}
