using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsoleApp2.Migrations
{
    public partial class Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestDTO",
                columns: table => new
                {visual
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDTO", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "failedRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestDTOID = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_failedRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_failedRequests_RequestDTO_RequestDTOID",
                        column: x => x.RequestDTOID,
                        principalTable: "RequestDTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "successfulRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestDTOID = table.Column<int>(type: "int", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_successfulRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_successfulRequests_RequestDTO_RequestDTOID",
                        column: x => x.RequestDTOID,
                        principalTable: "RequestDTO",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_failedRequests_RequestDTOID",
                table: "failedRequests",
                column: "RequestDTOID");

            migrationBuilder.CreateIndex(
                name: "IX_successfulRequests_RequestDTOID",
                table: "successfulRequests",
                column: "RequestDTOID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "failedRequests");

            migrationBuilder.DropTable(
                name: "successfulRequests");

            migrationBuilder.DropTable(
                name: "RequestDTO");
        }
    }
}
