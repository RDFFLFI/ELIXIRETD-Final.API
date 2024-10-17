using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class createFuelRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuelRegisters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Driver = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true),
                    Warehouse_ReceivingId = table.Column<int>(type: "int", nullable: true),
                    Liters = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Asset = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Odometer = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Company_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Company_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location_Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location_Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Added_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Updated_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is_Approve = table.Column<bool>(type: "bit", nullable: false),
                    Approve_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Approve_At = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Is_Transact = table.Column<bool>(type: "bit", nullable: true),
                    Transact_By = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Transact_At = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is_Active = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelRegisters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelRegisters_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FuelRegisters_WarehouseReceived_Warehouse_ReceivingId",
                        column: x => x.Warehouse_ReceivingId,
                        principalTable: "WarehouseReceived",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_MaterialId",
                table: "FuelRegisters",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelRegisters_Warehouse_ReceivingId",
                table: "FuelRegisters",
                column: "Warehouse_ReceivingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelRegisters");
        }
    }
}
