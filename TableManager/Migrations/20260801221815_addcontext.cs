using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class addcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MlCsvRow_MlCsv_MlCsvId",
                table: "MlCsvRow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MlCsvRow",
                table: "MlCsvRow");

            migrationBuilder.RenameTable(
                name: "MlCsvRow",
                newName: "MlCsvRows");

            migrationBuilder.RenameIndex(
                name: "IX_MlCsvRow_MlCsvId",
                table: "MlCsvRows",
                newName: "IX_MlCsvRows_MlCsvId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MlCsvRows",
                table: "MlCsvRows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MlCsvRows_MlCsv_MlCsvId",
                table: "MlCsvRows",
                column: "MlCsvId",
                principalTable: "MlCsv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MlCsvRows_MlCsv_MlCsvId",
                table: "MlCsvRows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MlCsvRows",
                table: "MlCsvRows");

            migrationBuilder.RenameTable(
                name: "MlCsvRows",
                newName: "MlCsvRow");

            migrationBuilder.RenameIndex(
                name: "IX_MlCsvRows_MlCsvId",
                table: "MlCsvRow",
                newName: "IX_MlCsvRow_MlCsvId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MlCsvRow",
                table: "MlCsvRow",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MlCsvRow_MlCsv_MlCsvId",
                table: "MlCsvRow",
                column: "MlCsvId",
                principalTable: "MlCsv",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
