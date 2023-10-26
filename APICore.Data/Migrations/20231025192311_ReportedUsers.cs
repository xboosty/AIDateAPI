using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICore.Data.Migrations
{
    public partial class ReportedUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReporterUserId = table.Column<int>(type: "int", nullable: false),
                    ReportedUserId = table.Column<int>(type: "int", nullable: false),
                    Coment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReporStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportedUsers_Users_ReportedUserId",
                        column: x => x.ReportedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportedUsers_Users_ReporterUserId",
                        column: x => x.ReporterUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportedUsers_ReportedUserId",
                table: "ReportedUsers",
                column: "ReportedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedUsers_ReporterUserId",
                table: "ReportedUsers",
                column: "ReporterUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportedUsers");
        }
    }
}
