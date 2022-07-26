using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class AddNewColumnLastUpdatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "TbTicketResponse",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "TbTicketResponse");
        }
    }
}
