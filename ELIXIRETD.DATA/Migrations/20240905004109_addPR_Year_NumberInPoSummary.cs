using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class addPR_Year_NumberInPoSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PR_Year_Number",
                table: "WarehouseReceived",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PR_Year_Number",
                table: "PoSummaries",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PR_Year_Number",
                table: "WarehouseReceived");

            migrationBuilder.DropColumn(
                name: "PR_Year_Number",
                table: "PoSummaries");
        }
    }
}
