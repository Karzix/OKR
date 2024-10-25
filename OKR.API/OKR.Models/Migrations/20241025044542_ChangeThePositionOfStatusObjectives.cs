using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class ChangeThePositionOfStatusObjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "UserObjectives");

            migrationBuilder.DropColumn(
                name: "status",
                table: "DepartmentObjectives");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Objectives",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "Objectives");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "UserObjectives",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "DepartmentObjectives",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
