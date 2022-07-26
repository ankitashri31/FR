using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class InitialMasterTablesCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbAuditHistoryMaster_TbAuditEvents_TbAuditEventsId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropTable(
                name: "TbAuditEvents");

            migrationBuilder.DropIndex(
                name: "IX_TbAuditHistoryMaster_TbAuditEventsId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropColumn(
                name: "TbAuditEventsId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.AddColumn<long>(
                name: "TicketMasterId",
                table: "TbAuditHistoryMaster",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TbChannelMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelName = table.Column<string>(nullable: true),
                    isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbChannelMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbOrganisationMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrganisationMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbTicketStatusMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTicketStatusMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbWaitingOnMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbWaitingOnMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbUsersMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    OrganisationId = table.Column<long>(nullable: false),
                    IsNotifyOnActive = table.Column<bool>(nullable: false),
                    IsNotifyOnUpdate = table.Column<bool>(nullable: false),
                    IsNotifyOnClose = table.Column<bool>(nullable: false),
                    LastLogOnDateTime = table.Column<DateTime>(nullable: true),
                    LastUpdatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbUsersMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbUsersMaster_TbOrganisationMaster_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "TbOrganisationMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbTicketMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<long>(nullable: false),
                    MatterNumber = table.Column<string>(nullable: true),
                    TicketName = table.Column<string>(nullable: true),
                    TicketNotes = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedByUserId = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    StatusId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTicketMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbTicketMaster_TbChannelMaster_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TbChannelMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TbTicketMaster_TbTicketStatusMaster_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TbTicketStatusMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TbTicketDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketMasterId = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    FileData = table.Column<byte[]>(nullable: true),
                    CreatedByUserId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTicketDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbTicketDocuments_TbTicketMaster_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TbTicketMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TbTicketResponse",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketMasterId = table.Column<long>(nullable: false),
                    WaitingOnId = table.Column<long>(nullable: false),
                    LinfoxNotes = table.Column<string>(nullable: true),
                    MedilawNotes = table.Column<string>(nullable: true),
                    HWLENotes = table.Column<string>(nullable: true),
                    StatusId = table.Column<long>(nullable: true),
                    UserMasterId = table.Column<long>(nullable: false),
                    OrganisationId = table.Column<long>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    FileData = table.Column<byte[]>(nullable: true),
                    CreatedByUserId = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedBy = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTicketResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbTicketResponse_TbTicketStatusMaster_StatusId",
                        column: x => x.StatusId,
                        principalTable: "TbTicketStatusMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TbTicketResponse_TbTicketMaster_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TbTicketMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbTicketResponse_TbUsersMaster_UserMasterId",
                        column: x => x.UserMasterId,
                        principalTable: "TbUsersMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TbTicketResponse_TbWaitingOnMaster_WaitingOnId",
                        column: x => x.WaitingOnId,
                        principalTable: "TbWaitingOnMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbAuditHistoryMaster_TicketMasterId",
                table: "TbAuditHistoryMaster",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbAuditHistoryMaster_UserMasterId",
                table: "TbAuditHistoryMaster",
                column: "UserMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketDocuments_TicketMasterId",
                table: "TbTicketDocuments",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketMaster_ChannelId",
                table: "TbTicketMaster",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketMaster_StatusId",
                table: "TbTicketMaster",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_StatusId",
                table: "TbTicketResponse",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_TicketMasterId",
                table: "TbTicketResponse",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_UserMasterId",
                table: "TbTicketResponse",
                column: "UserMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_WaitingOnId",
                table: "TbTicketResponse",
                column: "WaitingOnId");

            migrationBuilder.CreateIndex(
                name: "IX_TbUsersMaster_OrganisationId",
                table: "TbUsersMaster",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbAuditHistoryMaster_TbTicketMaster_TicketMasterId",
                table: "TbAuditHistoryMaster",
                column: "TicketMasterId",
                principalTable: "TbTicketMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TbAuditHistoryMaster_TbUsersMaster_UserMasterId",
                table: "TbAuditHistoryMaster",
                column: "UserMasterId",
                principalTable: "TbUsersMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbAuditHistoryMaster_TbTicketMaster_TicketMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TbAuditHistoryMaster_TbUsersMaster_UserMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropTable(
                name: "TbTicketDocuments");

            migrationBuilder.DropTable(
                name: "TbTicketResponse");

            migrationBuilder.DropTable(
                name: "TbTicketMaster");

            migrationBuilder.DropTable(
                name: "TbUsersMaster");

            migrationBuilder.DropTable(
                name: "TbWaitingOnMaster");

            migrationBuilder.DropTable(
                name: "TbChannelMaster");

            migrationBuilder.DropTable(
                name: "TbTicketStatusMaster");

            migrationBuilder.DropTable(
                name: "TbOrganisationMaster");

            migrationBuilder.DropIndex(
                name: "IX_TbAuditHistoryMaster_TicketMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropIndex(
                name: "IX_TbAuditHistoryMaster_UserMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropColumn(
                name: "TicketMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.AddColumn<long>(
                name: "ApplicationId",
                table: "TbAuditHistoryMaster",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TbAuditEventsId",
                table: "TbAuditHistoryMaster",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TbAuditEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuditEvent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbAuditEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbAuditHistoryMaster_TbAuditEventsId",
                table: "TbAuditHistoryMaster",
                column: "TbAuditEventsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbAuditHistoryMaster_TbAuditEvents_TbAuditEventsId",
                table: "TbAuditHistoryMaster",
                column: "TbAuditEventsId",
                principalTable: "TbAuditEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
