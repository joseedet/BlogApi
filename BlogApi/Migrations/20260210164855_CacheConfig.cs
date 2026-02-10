using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    /// <inheritdoc />
    public partial class CacheConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CacheConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpiracionPostsSegundos = table.Column<int>(type: "int", nullable: false),
                    ExpiracionComentariosSegundos = table.Column<int>(type: "int", nullable: false),
                    ExpiracionDashboardSegundos = table.Column<int>(type: "int", nullable: false),
                    ExpiracionRolesSegundos = table.Column<int>(type: "int", nullable: false),
                    ExpiracionPermisosSegundos = table.Column<int>(type: "int", nullable: false),
                    ExpiracionUsuariosSegundos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CacheConfig", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CacheConfig");
        }
    }
}
