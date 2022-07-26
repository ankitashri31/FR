using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Added_Cols_TickerResponseDocumentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "TbTicketResponseDocuments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TbTicketResponseDocuments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TbTicketResponseDocuments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TbTicketResponseDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TbTicketResponseDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TbTicketResponseDocuments");
        }
    }
}
