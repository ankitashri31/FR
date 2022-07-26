using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Added_DeletedBy_TicketMaster_Documents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "TbTicketMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "TbTicketDocuments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TbTicketDocuments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "TbTicketMaster");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TbTicketDocuments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TbTicketDocuments");
        }
    }
}
