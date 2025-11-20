using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GovernmentEntityId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GovernmentEntityId",
                table: "Roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GovernmentEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernmentEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GovernmentEntityId",
                table: "Users",
                column: "GovernmentEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GovernmentEntityId",
                table: "Roles",
                column: "GovernmentEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_GovernmentEntity_GovernmentEntityId",
                table: "Roles",
                column: "GovernmentEntityId",
                principalTable: "GovernmentEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_GovernmentEntity_GovernmentEntityId",
                table: "Users",
                column: "GovernmentEntityId",
                principalTable: "GovernmentEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_GovernmentEntity_GovernmentEntityId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_GovernmentEntity_GovernmentEntityId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "GovernmentEntity");

            migrationBuilder.DropIndex(
                name: "IX_Users_GovernmentEntityId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Roles_GovernmentEntityId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GovernmentEntityId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GovernmentEntityId",
                table: "Roles");
        }
    }
}
