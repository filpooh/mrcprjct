using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class AddRefUserMlCsv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileCsvs_AspNetUsers_UserId",
                table: "FileCsvs");

            migrationBuilder.DropColumn(
                name: "MaxJson",
                table: "FileCsvs");

            migrationBuilder.DropColumn(
                name: "MinJson",
                table: "FileCsvs");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MlCsv",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MlCsv_UserId",
                table: "MlCsv",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileCsvs_AspNetUsers_UserId",
                table: "FileCsvs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MlCsv_AspNetUsers_UserId",
                table: "MlCsv",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileCsvs_AspNetUsers_UserId",
                table: "FileCsvs");

            migrationBuilder.DropForeignKey(
                name: "FK_MlCsv_AspNetUsers_UserId",
                table: "MlCsv");

            migrationBuilder.DropIndex(
                name: "IX_MlCsv_UserId",
                table: "MlCsv");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MlCsv");

            migrationBuilder.AddColumn<string>(
                name: "MaxJson",
                table: "FileCsvs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MinJson",
                table: "FileCsvs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FileCsvs_AspNetUsers_UserId",
                table: "FileCsvs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
