using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class mlcsvrow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderJson",
                table: "MlCsv",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MlCsvRow",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MlCsvId = table.Column<int>(type: "int", nullable: false),
                    NumeroRiga = table.Column<int>(type: "int", nullable: false),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MlCsvRow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MlCsvRow_MlCsv_MlCsvId",
                        column: x => x.MlCsvId,
                        principalTable: "MlCsv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MlCsvRow_MlCsvId",
                table: "MlCsvRow",
                column: "MlCsvId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MlCsvRow");

            migrationBuilder.DropColumn(
                name: "HeaderJson",
                table: "MlCsv");
        }
    }
}
