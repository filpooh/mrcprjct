using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class minorchangemlcsv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Statistics_MlCsvId",
                table: "Statistics");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "MlCsv",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_MlCsvId",
                table: "Statistics",
                column: "MlCsvId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Statistics_MlCsvId",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "type",
                table: "MlCsv");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_MlCsvId",
                table: "Statistics",
                column: "MlCsvId");
        }
    }
}
