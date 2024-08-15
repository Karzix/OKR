using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class addColumCompletionRateForProgressUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KeyresultCompletionRate",
                table: "ProgressUpdates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObjectivesCompletionRate",
                table: "ProgressUpdates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyresultCompletionRate",
                table: "ProgressUpdates");

            migrationBuilder.DropColumn(
                name: "ObjectivesCompletionRate",
                table: "ProgressUpdates");
        }
    }
}
