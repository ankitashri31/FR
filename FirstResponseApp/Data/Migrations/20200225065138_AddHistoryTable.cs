using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class AddHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbAuditEvents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditEvent = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAuditEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbAuditHistoryMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditEventTypeId = table.Column<long>(nullable: true),
                    UserMasterId = table.Column<long>(nullable: true),
                    UserEmailAddress = table.Column<string>(nullable: true),
                    RecordedDate = table.Column<DateTime>(nullable: true),
                    BrowserVersion = table.Column<string>(nullable: true),
                    AuditEventName = table.Column<string>(nullable: true),
                    AuditEventDescription = table.Column<string>(nullable: true),
                    ApplicationId = table.Column<long>(nullable: true),
                    TbAuditEventsId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAuditHistoryMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbAuditHistoryMaster_TbAuditEvents_TbAuditEventsId",
                        column: x => x.TbAuditEventsId,
                        principalTable: "TbAuditEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbAuditHistoryMaster_TbAuditEventsId",
                table: "TbAuditHistoryMaster",
                column: "TbAuditEventsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbAuditHistoryMaster");

            migrationBuilder.DropTable(
                name: "TbAuditEvents");
        }
    }
}
