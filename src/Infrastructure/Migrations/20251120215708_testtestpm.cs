using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class testtestpm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId",
                table: "PasswordSetupOtps");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId1",
                table: "PasswordSetupOtps");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId",
                table: "PasswordSetupOtps",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId1",
                table: "PasswordSetupOtps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId",
                table: "PasswordSetupOtps");

            migrationBuilder.DropForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId1",
                table: "PasswordSetupOtps");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId",
                table: "PasswordSetupOtps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordSetupOtps_Users_UserId1",
                table: "PasswordSetupOtps",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
