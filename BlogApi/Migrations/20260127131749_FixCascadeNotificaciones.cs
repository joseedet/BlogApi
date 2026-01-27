using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    public partial class FixCascadeNotificaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar las FK antiguas con CASCADE
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones"
            );

            // Crear las FK nuevas con RESTRICT
            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones",
                column: "UsuarioOrigenId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir a CASCADE si hiciera falta
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones",
                column: "UsuarioOrigenId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
