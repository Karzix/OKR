using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_Column_that_defines_the_time : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StringOfDays",
                table: "Objectives",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Objectives",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "KeyResults",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StringOfDays",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "KeyResults");
        }
    }
}
