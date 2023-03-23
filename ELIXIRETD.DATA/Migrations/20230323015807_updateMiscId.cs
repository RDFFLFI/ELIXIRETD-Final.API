using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updateMiscId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiscellanousReceiptId",
                table: "WarehouseReceived",
                newName: "MiscellaneousReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MiscellaneousReceiptId",
                table: "WarehouseReceived",
                newName: "MiscellanousReceiptId");
        }
    }
}
