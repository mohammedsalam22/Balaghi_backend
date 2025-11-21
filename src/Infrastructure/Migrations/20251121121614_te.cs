using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class te : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CitizenId = table.Column<Guid>(type: "uuid", nullable: false),
                    CitizenId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    AgencyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AgencyId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    ComplaintType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_GovernmentAgencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "GovernmentAgencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_GovernmentAgencies_AgencyId1",
                        column: x => x.AgencyId1,
                        principalTable: "GovernmentAgencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaints_Users_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_Users_CitizenId1",
                        column: x => x.CitizenId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComplaintAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComplaintId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComplaintId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplaintAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComplaintAttachments_Complaints_ComplaintId",
                        column: x => x.ComplaintId,
                        principalTable: "Complaints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComplaintAttachments_Complaints_ComplaintId1",
                        column: x => x.ComplaintId1,
                        principalTable: "Complaints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId",
                table: "ComplaintAttachments",
                column: "ComplaintId");

            migrationBuilder.CreateIndex(
                name: "IX_ComplaintAttachments_ComplaintId1",
                table: "ComplaintAttachments",
                column: "ComplaintId1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AgencyId",
                table: "Complaints",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AgencyId1",
                table: "Complaints",
                column: "AgencyId1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CitizenId",
                table: "Complaints",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_CitizenId1",
                table: "Complaints",
                column: "CitizenId1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TrackingNumber",
                table: "Complaints",
                column: "TrackingNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComplaintAttachments");

            migrationBuilder.DropTable(
                name: "Complaints");
        }
    }
}
