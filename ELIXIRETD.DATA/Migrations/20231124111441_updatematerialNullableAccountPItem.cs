using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELIXIRETD.DATA.Migrations
{
    public partial class updatematerialNullableAccountPItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials");

            migrationBuilder.AlterColumn<int>(
                name: "AccountTitleId",
                table: "Materials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials",
                column: "AccountTitleId",
                principalTable: "AccountTitles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials");

            migrationBuilder.AlterColumn<int>(
                name: "AccountTitleId",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_AccountTitles_AccountTitleId",
                table: "Materials",
                column: "AccountTitleId",
                principalTable: "AccountTitles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
