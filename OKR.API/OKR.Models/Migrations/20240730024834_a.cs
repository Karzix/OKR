using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentObjectives_Objective_ObjectiveId",
                table: "DepartmentObjectives");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyResults_Objective_ObjectiveId",
                table: "KeyResults");

            migrationBuilder.DropForeignKey(
                name: "FK_UserObjectives_Objective_ObjectiveId",
                table: "UserObjectives");

            migrationBuilder.DropTable(
                name: "Objective");

            migrationBuilder.RenameColumn(
                name: "ObjectiveId",
                table: "UserObjectives",
                newName: "ObjectivesId");

            migrationBuilder.RenameIndex(
                name: "IX_UserObjectives_ObjectiveId",
                table: "UserObjectives",
                newName: "IX_UserObjectives_ObjectivesId");

            migrationBuilder.RenameColumn(
                name: "ObjectiveId",
                table: "KeyResults",
                newName: "ObjectivesId");

            migrationBuilder.RenameIndex(
                name: "IX_KeyResults_ObjectiveId",
                table: "KeyResults",
                newName: "IX_KeyResults_ObjectivesId");

            migrationBuilder.RenameColumn(
                name: "ObjectiveId",
                table: "DepartmentObjectives",
                newName: "ObjectivesId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentObjectives_ObjectiveId",
                table: "DepartmentObjectives",
                newName: "IX_DepartmentObjectives_ObjectivesId");

            migrationBuilder.CreateTable(
                name: "Objectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifiedby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objectives_TargetType_TargetTypeId",
                        column: x => x.TargetTypeId,
                        principalTable: "TargetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_TargetTypeId",
                table: "Objectives",
                column: "TargetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentObjectives_Objectives_ObjectivesId",
                table: "DepartmentObjectives",
                column: "ObjectivesId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyResults_Objectives_ObjectivesId",
                table: "KeyResults",
                column: "ObjectivesId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserObjectives_Objectives_ObjectivesId",
                table: "UserObjectives",
                column: "ObjectivesId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DepartmentObjectives_Objectives_ObjectivesId",
                table: "DepartmentObjectives");

            migrationBuilder.DropForeignKey(
                name: "FK_KeyResults_Objectives_ObjectivesId",
                table: "KeyResults");

            migrationBuilder.DropForeignKey(
                name: "FK_UserObjectives_Objectives_ObjectivesId",
                table: "UserObjectives");

            migrationBuilder.DropTable(
                name: "Objectives");

            migrationBuilder.RenameColumn(
                name: "ObjectivesId",
                table: "UserObjectives",
                newName: "ObjectiveId");

            migrationBuilder.RenameIndex(
                name: "IX_UserObjectives_ObjectivesId",
                table: "UserObjectives",
                newName: "IX_UserObjectives_ObjectiveId");

            migrationBuilder.RenameColumn(
                name: "ObjectivesId",
                table: "KeyResults",
                newName: "ObjectiveId");

            migrationBuilder.RenameIndex(
                name: "IX_KeyResults_ObjectivesId",
                table: "KeyResults",
                newName: "IX_KeyResults_ObjectiveId");

            migrationBuilder.RenameColumn(
                name: "ObjectivesId",
                table: "DepartmentObjectives",
                newName: "ObjectiveId");

            migrationBuilder.RenameIndex(
                name: "IX_DepartmentObjectives_ObjectivesId",
                table: "DepartmentObjectives",
                newName: "IX_DepartmentObjectives_ObjectiveId");

            migrationBuilder.CreateTable(
                name: "Objective",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifiedby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDay = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objective", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objective_TargetType_TargetTypeId",
                        column: x => x.TargetTypeId,
                        principalTable: "TargetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objective_TargetTypeId",
                table: "Objective",
                column: "TargetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DepartmentObjectives_Objective_ObjectiveId",
                table: "DepartmentObjectives",
                column: "ObjectiveId",
                principalTable: "Objective",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KeyResults_Objective_ObjectiveId",
                table: "KeyResults",
                column: "ObjectiveId",
                principalTable: "Objective",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserObjectives_Objective_ObjectiveId",
                table: "UserObjectives",
                column: "ObjectiveId",
                principalTable: "Objective",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
