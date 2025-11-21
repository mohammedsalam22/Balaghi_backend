using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tsded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
                table: "ComplaintAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_CitizenId1",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_CitizenId1",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_ComplaintAttachments_ComplaintId1",
                table: "ComplaintAttachments");

            migrationBuilder.DropColumn(
                name: "CitizenId1",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "ComplaintId1",
                table: "ComplaintAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CitizenId1",
                table: "Complaints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ComplaintId1",
                table: "ComplaintAttachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CitizenId1",
                table: "Complaints",
                column: "CitizenId1");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId1",
                table: "ComplaintAttachments",
                column: "ComplaintId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
                table: "ComplaintAttachments",
                column: "ComplaintId1",
                principalTable: "Complaints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_CitizenId1",
                table: "Complaints",
                column: "CitizenId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
