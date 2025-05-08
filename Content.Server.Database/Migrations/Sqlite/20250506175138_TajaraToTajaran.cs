using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class TajaraToTajaran : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "profile",
                keyColumn: "species",
                keyValue: "Tajara",
                column: "species",
                value: "Tajaran");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "profile",
                keyColumn: "species",
                keyValue: "Tajaran",
                column: "species",
                value: "Tajara");
        }
    }
}
