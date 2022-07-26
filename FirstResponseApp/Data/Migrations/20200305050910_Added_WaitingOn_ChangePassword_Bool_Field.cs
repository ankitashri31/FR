using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Added_WaitingOn_ChangePassword_Bool_Field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "WaitingOnOrganisationId",
                table: "TbTicketMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsFirstTimeChangedPassword",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketMaster_WaitingOnOrganisationId",
                table: "TbTicketMaster",
                column: "WaitingOnOrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbTicketMaster_TbOrganisationMaster_WaitingOnOrganisationId",
                table: "TbTicketMaster",
                column: "WaitingOnOrganisationId",
                principalTable: "TbOrganisationMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbTicketMaster_TbOrganisationMaster_WaitingOnOrganisationId",
                table: "TbTicketMaster");

            migrationBuilder.DropIndex(
                name: "IX_TbTicketMaster_WaitingOnOrganisationId",
                table: "TbTicketMaster");

            migrationBuilder.DropColumn(
                name: "WaitingOnOrganisationId",
                table: "TbTicketMaster");

            migrationBuilder.DropColumn(
                name: "IsFirstTimeChangedPassword",
                table: "AspNetUsers");
        }
    }
}
