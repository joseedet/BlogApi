using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetToken1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpCreacion",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IpUso",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgentCreacion",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgentUso",
                table: "PasswordResetTokens",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpCreacion",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "IpUso",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "UserAgentCreacion",
                table: "PasswordResetTokens");

            migrationBuilder.DropColumn(
                name: "UserAgentUso",
                table: "PasswordResetTokens");
        }
    }
}
