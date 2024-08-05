using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class addColumForProgressUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewPoint",
                table: "ProgressUpdates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OldPoint",
                table: "ProgressUpdates",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPoint",
                table: "ProgressUpdates");

            migrationBuilder.DropColumn(
                name: "OldPoint",
                table: "ProgressUpdates");
        }
    }
}
