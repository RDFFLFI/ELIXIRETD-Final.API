using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class removeFuel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Fuels_FuelId",
                table: "FuelRegisters");

            migrationBuilder.DropTable(
                name: "Fuels");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_FuelId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "FuelId",
                table: "FuelRegisters");

            migrationBuilder.AlterColumn<decimal>(
                name: "Liters",
                table: "FuelRegisters",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "FuelRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "FuelRegisters");

            migrationBuilder.AlterColumn<decimal>(
                name: "Liters",
                table: "FuelRegisters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuelId",
                table: "FuelRegisters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Fuels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Added_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Modified_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "IX_FuelRegisters_FuelId",
                table: "FuelRegisters",
                column: "FuelId");

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
    }
}
