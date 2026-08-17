using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TableManager.Migrations
{
    /// <inheritdoc />
    public partial class minorchangestat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Coef",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DurationSeconds",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Statistics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<float>(
                name: "Intercept",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Mse",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "R2",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Rmse",
                table: "Statistics",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Statistics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coef",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Intercept",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Mse",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "R2",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "Rmse",
                table: "Statistics");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Statistics");
        }
    }
}
