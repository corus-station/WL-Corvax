using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class OocText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "ooc_text",
                table: "profile");

            migrationBuilder.AddColumn<int>(
                name: "height",
                table: "profile",
                type: "INTEGER",
                nullable: false,
                defaultValue: 130);

            migrationBuilder.AddColumn<string>(
                name: "ooc_text",
                table: "profile",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "height",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "ooc_text",
                table: "profile");
        }
    }
}
