using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createFuelDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelRegisters_WarehouseReceived_Warehouse_ReceivingId",
                table: "FuelRegisters");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters");

            migrationBuilder.DropIndex(
                name: "IX_FuelRegisters_Warehouse_ReceivingId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Asset",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Liters",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Odometer",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Plate_No",
                table: "FuelRegisters");

            migrationBuilder.DropColumn(
                name: "Warehouse_ReceivingId",
                table: "FuelRegisters");

            migrationBuilder.CreateTable(
                name: "FuelRegisterDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelRegisterId = table.Column<int>(type: "int", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Warehouse_ReceivingId = table.Column<int>(type: "int", nullable: true),
                    Liters = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Asset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Odometer = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Added_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRegisterDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelRegisterDetails_FuelRegisters_FuelRegisterId",
                        column: x => x.FuelRegisterId,
                        principalTable: "FuelRegisters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelRegisterDetails_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FuelRegisterDetails_WarehouseReceived_Warehouse_ReceivingId",
                        column: x => x.Warehouse_ReceivingId,
                        principalTable: "WarehouseReceived",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisterDetails_FuelRegisterId",
                table: "FuelRegisterDetails",
                column: "FuelRegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisterDetails_MaterialId",
                table: "FuelRegisterDetails",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisterDetails_Warehouse_ReceivingId",
                table: "FuelRegisterDetails",
                column: "Warehouse_ReceivingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelRegisterDetails");

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Liters",
                table: "FuelRegisters",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "FuelRegisters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Odometer",
                table: "FuelRegisters",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plate_No",
                table: "FuelRegisters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Warehouse_ReceivingId",
                table: "FuelRegisters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_Warehouse_ReceivingId",
                table: "FuelRegisters",
                column: "Warehouse_ReceivingId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_Materials_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FuelRegisters_WarehouseReceived_Warehouse_ReceivingId",
                table: "FuelRegisters",
                column: "Warehouse_ReceivingId",
                principalTable: "WarehouseReceived",
                principalColumn: "Id");
        }
    }
}
