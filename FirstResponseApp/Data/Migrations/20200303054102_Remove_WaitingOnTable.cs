using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Remove_WaitingOnTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbTicketResponse_TbWaitingOnMaster_WaitingOnId",
                table: "TbTicketResponse");

            migrationBuilder.DropTable(
                name: "TbWaitingOnMaster");

            migrationBuilder.DropIndex(
                name: "IX_TbTicketResponse_WaitingOnId",
                table: "TbTicketResponse");

            migrationBuilder.DropColumn(
                name: "WaitingOnId",
                table: "TbTicketResponse");

            migrationBuilder.AddColumn<long>(
                name: "WaitingOnOrganisationId",
                table: "TbTicketResponse",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_WaitingOnOrganisationId",
                table: "TbTicketResponse",
                column: "WaitingOnOrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbTicketResponse_TbOrganisationMaster_WaitingOnOrganisationId",
                table: "TbTicketResponse",
                column: "WaitingOnOrganisationId",
                principalTable: "TbOrganisationMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbTicketResponse_TbOrganisationMaster_WaitingOnOrganisationId",
                table: "TbTicketResponse");

            migrationBuilder.DropIndex(
                name: "IX_TbTicketResponse_WaitingOnOrganisationId",
                table: "TbTicketResponse");

            migrationBuilder.DropColumn(
                name: "WaitingOnOrganisationId",
                table: "TbTicketResponse");

            migrationBuilder.AddColumn<long>(
                name: "WaitingOnId",
                table: "TbTicketResponse",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TbWaitingOnMaster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbWaitingOnMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponse_WaitingOnId",
                table: "TbTicketResponse",
                column: "WaitingOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_TbTicketResponse_TbWaitingOnMaster_WaitingOnId",
                table: "TbTicketResponse",
                column: "WaitingOnId",
                principalTable: "TbWaitingOnMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
