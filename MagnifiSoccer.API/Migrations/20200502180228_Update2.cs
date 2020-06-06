using Microsoft.EntityFrameworkCore.Migrations;

namespace MagnifiSoccer.API.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Game_GameId",
                table: "GamePlayer");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "GamePlayer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Game_GameId",
                table: "GamePlayer",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayer_Game_GameId",
                table: "GamePlayer");

            migrationBuilder.AlterColumn<int>(
                name: "GameId",
                table: "GamePlayer",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayer_Game_GameId",
                table: "GamePlayer",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
