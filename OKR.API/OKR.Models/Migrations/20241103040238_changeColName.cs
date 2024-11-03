using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class changeColName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StringOfDays",
                table: "Objectives",
                newName: "Period");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "Objectives",
                newName: "StringOfDays");
        }
    }
}
