using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Added_ClosedBy_LastModifiedBy_Cols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedOn",
                table: "TbTicketMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedOn",
                table: "TbTicketMaster");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "AspNetUsers");
        }
    }
}
