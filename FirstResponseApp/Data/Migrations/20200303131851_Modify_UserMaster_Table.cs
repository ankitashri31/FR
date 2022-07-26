using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Modify_UserMaster_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbAuditHistoryMaster_TbUsersMaster_UserMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TbTicketResponse_TbUsersMaster_UserMasterId",
                table: "TbTicketResponse");

            migrationBuilder.DropTable(
                name: "TbUsersMaster");

            migrationBuilder.AlterColumn<string>(
                name: "UserMasterId",
                table: "TbTicketResponse",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "UserMasterId",
                table: "TbAuditHistoryMaster",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyOnActiveTicket",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyOnCloseTicket",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNotifyOnUpdateTicket",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogOnDateTime",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrganisationId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TbOrganisationMaster_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId",
                principalTable: "TbOrganisationMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TbAuditHistoryMaster_AspNetUsers_UserMasterId",
                table: "TbAuditHistoryMaster",
                column: "UserMasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TbTicketResponse_AspNetUsers_UserMasterId",
                table: "TbTicketResponse",
                column: "UserMasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TbOrganisationMaster_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TbAuditHistoryMaster_AspNetUsers_UserMasterId",
                table: "TbAuditHistoryMaster");

            migrationBuilder.DropForeignKey(
                name: "FK_TbTicketResponse_AspNetUsers_UserMasterId",
                table: "TbTicketResponse");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsNotifyOnActiveTicket",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsNotifyOnCloseTicket",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsNotifyOnUpdateTicket",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLogOnDateTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<long>(
                name: "UserMasterId",
                table: "TbTicketResponse",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserMasterId",
                table: "TbAuditHistoryMaster",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TbUsersMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsNotifyOnActive = table.Column<bool>(type: "bit", nullable: false),
                    IsNotifyOnClose = table.Column<bool>(type: "bit", nullable: false),
                    IsNotifyOnUpdate = table.Column<bool>(type: "bit", nullable: false),
                    LastLogOnDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganisationId = table.Column<long>(type: "bigint", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_TbUsersMaster_OrganisationId",
                table: "TbUsersMaster",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbAuditHistoryMaster_TbUsersMaster_UserMasterId",
                table: "TbAuditHistoryMaster",
                column: "UserMasterId",
                principalTable: "TbUsersMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TbTicketResponse_TbUsersMaster_UserMasterId",
                table: "TbTicketResponse",
                column: "UserMasterId",
                principalTable: "TbUsersMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
