using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    /// <inheritdoc />
    public partial class LogAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogAdmins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioAdminId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioAfectadoId = table.Column<int>(type: "int", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Detalles = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogAdmins_Usuarios_UsuarioAdminId",
                        column: x => x.UsuarioAdminId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LogAdmins_Usuarios_UsuarioAfectadoId",
                        column: x => x.UsuarioAfectadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogAdmins_UsuarioAdminId",
                table: "LogAdmins",
                column: "UsuarioAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LogAdmins_UsuarioAfectadoId",
                table: "LogAdmins",
                column: "UsuarioAfectadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogAdmins");
        }
    }
}
