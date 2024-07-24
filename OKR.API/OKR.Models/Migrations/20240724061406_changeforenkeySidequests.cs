using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class changeforenkeySidequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sidequests_Objective_ObjectiveId",
                table: "Sidequests");

            migrationBuilder.RenameColumn(
                name: "ObjectiveId",
                table: "Sidequests",
                newName: "KeyResultsId");

            migrationBuilder.RenameIndex(
                name: "IX_Sidequests_ObjectiveId",
                table: "Sidequests",
                newName: "IX_Sidequests_KeyResultsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sidequests_KeyResults_KeyResultsId",
                table: "Sidequests",
                column: "KeyResultsId",
                principalTable: "KeyResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sidequests_KeyResults_KeyResultsId",
                table: "Sidequests");

            migrationBuilder.RenameColumn(
                name: "KeyResultsId",
                table: "Sidequests",
                newName: "ObjectiveId");

            migrationBuilder.RenameIndex(
                name: "IX_Sidequests_KeyResultsId",
                table: "Sidequests",
                newName: "IX_Sidequests_ObjectiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sidequests_Objective_ObjectiveId",
                table: "Sidequests",
                column: "ObjectiveId",
                principalTable: "Objective",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
