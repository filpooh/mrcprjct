using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class _2columnSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatoVerbose",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatoVerbose",
                table: "Settings");
        }
    }
}
