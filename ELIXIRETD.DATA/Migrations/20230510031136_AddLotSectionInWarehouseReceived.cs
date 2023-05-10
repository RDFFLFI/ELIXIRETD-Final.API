using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class AddLotSectionInWarehouseReceived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LotCategory",
                table: "WarehouseReceived",
                newName: "LotSection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LotSection",
                table: "WarehouseReceived",
                newName: "LotCategory");
        }
    }
}
