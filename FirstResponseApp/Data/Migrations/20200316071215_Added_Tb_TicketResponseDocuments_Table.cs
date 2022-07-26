using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FirstResponseApp.Data.Migrations
{
    public partial class Added_Tb_TicketResponseDocuments_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbTicketResponseDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketMasterId = table.Column<long>(nullable: false),
                    TicketResponseId = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    FileData = table.Column<byte[]>(nullable: true),
                    CreatedByUserId = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbTicketResponseDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbTicketResponseDocuments_TbTicketMaster_TicketMasterId",
                        column: x => x.TicketMasterId,
                        principalTable: "TbTicketMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TbTicketResponseDocuments_TbTicketResponse_TicketResponseId",
                        column: x => x.TicketResponseId,
                        principalTable: "TbTicketResponse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponseDocuments_TicketMasterId",
                table: "TbTicketResponseDocuments",
                column: "TicketMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_TbTicketResponseDocuments_TicketResponseId",
                table: "TbTicketResponseDocuments",
                column: "TicketResponseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbTicketResponseDocuments");
        }
    }
}
