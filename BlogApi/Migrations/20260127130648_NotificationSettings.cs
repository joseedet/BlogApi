using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    public partial class NotificationSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones"
            );

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones"
            );

            migrationBuilder.DropColumn(name: "UsuarioId", table: "Notificaciones");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Notificaciones",
                newName: "FechaCreacion"
            );

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendEmailOnComment = table.Column<bool>(type: "bit", nullable: false),
                    SendEmailOnAdminMessage = table.Column<bool>(type: "bit", nullable: false),
                    SendEmailOnSystemAlert = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioOrigenId",
                table: "Notificaciones",
                column: "UsuarioOrigenId"
            );

            // 🔥 CORREGIDO: RESTRICT en lugar de CASCADE
            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones",
                column: "UsuarioDestinoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones",
                column: "UsuarioOrigenId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioDestinoId",
                table: "Notificaciones"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioOrigenId",
                table: "Notificaciones"
            );

            migrationBuilder.DropTable(name: "NotificationSettings");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_UsuarioDestinoId",
                table: "Notificaciones"
            );

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_UsuarioOrigenId",
                table: "Notificaciones"
            );

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Notificaciones",
                newName: "Fecha"
            );

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Notificaciones",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId"
            );

            // Restaurar CASCADE solo si haces rollback
            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
