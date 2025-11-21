using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_GovernmentAgencies_AgencyId1",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_AgencyId1",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "AgencyId1",
                table: "Complaints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgencyId1",
                table: "Complaints",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AgencyId1",
                table: "Complaints",
                column: "AgencyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_GovernmentAgencies_AgencyId1",
                table: "Complaints",
                column: "AgencyId1",
                principalTable: "GovernmentAgencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
