using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class changesettingmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_FileCsvs_FileId",
                table: "Settings");

            migrationBuilder.DropIndex(
                name: "IX_Settings_FileId",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "Settings",
                newName: "RegressionType");

            migrationBuilder.AddColumn<string>(
                name: "DummyColumn",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "MlId",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NormalizeColumn",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_MlId",
                table: "Settings",
                column: "MlId");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_MlCsv_MlId",
                table: "Settings",
                column: "MlId",
                principalTable: "MlCsv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Settings_MlCsv_MlId",
                table: "Settings");

            migrationBuilder.DropIndex(
                name: "IX_Settings_MlId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DummyColumn",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MlId",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "NormalizeColumn",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "RegressionType",
                table: "Settings",
                newName: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_FileId",
                table: "Settings",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Settings_FileCsvs_FileId",
                table: "Settings",
                column: "FileId",
                principalTable: "FileCsvs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
