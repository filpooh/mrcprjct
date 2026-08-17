using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class removebrokenmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cell");

            migrationBuilder.DropTable(
                name: "TableRow");

            migrationBuilder.DropTable(
                name: "TableProp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TableProp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableProp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableProp_FileCsvs_FileId",
                        column: x => x.FileId,
                        principalTable: "FileCsvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableRow",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TablePropId = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableRow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableRow_Settings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Settings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TableRow_TableProp_TablePropId",
                        column: x => x.TablePropId,
                        principalTable: "TableProp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cell",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableRowId = table.Column<int>(type: "int", nullable: false),
                    ColumnIndex = table.Column<int>(type: "int", nullable: false),
                    SettingId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cell", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cell_Settings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "Settings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cell_TableRow_TableRowId",
                        column: x => x.TableRowId,
                        principalTable: "TableRow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cell_SettingId",
                table: "Cell",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cell_TableRowId",
                table: "Cell",
                column: "TableRowId");

            migrationBuilder.CreateIndex(
                name: "IX_TableProp_FileId",
                table: "TableProp",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_TableRow_SettingId",
                table: "TableRow",
                column: "SettingId");

            migrationBuilder.CreateIndex(
                name: "IX_TableRow_TablePropId",
                table: "TableRow",
                column: "TablePropId");
        }
    }
}
