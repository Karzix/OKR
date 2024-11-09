using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class changeDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTarget_DepartmentObjectives_DepartmentObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTarget_UserObjectives_UserObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropTable(
                name: "DepartmentObjectives");

            migrationBuilder.DropTable(
                name: "Sidequests");

            migrationBuilder.DropTable(
                name: "UserObjectives");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTarget_DepartmentObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTarget_UserObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropColumn(
                name: "Quarter",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "DepartmentObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropColumn(
                name: "UserObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Objectives",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Objectives",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDay",
                table: "Objectives",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Objectives",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUserObjectives",
                table: "Objectives",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDay",
                table: "Objectives",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Percentage",
                table: "KeyResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ObjectivesId",
                table: "EvaluateTarget",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "AspNetUsers",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_ApplicationUserId",
                table: "Objectives",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_DepartmentId",
                table: "Objectives",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTarget_ObjectivesId",
                table: "EvaluateTarget",
                column: "ObjectivesId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTarget_Objectives_ObjectivesId",
                table: "EvaluateTarget",
                column: "ObjectivesId",
                principalTable: "Objectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_AspNetUsers_ApplicationUserId",
                table: "Objectives",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_Department_DepartmentId",
                table: "Objectives",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EvaluateTarget_Objectives_ObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_AspNetUsers_ApplicationUserId",
                table: "Objectives");

            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_Department_DepartmentId",
                table: "Objectives");

            migrationBuilder.DropIndex(
                name: "IX_Objectives_ApplicationUserId",
                table: "Objectives");

            migrationBuilder.DropIndex(
                name: "IX_Objectives_DepartmentId",
                table: "Objectives");

            migrationBuilder.DropIndex(
                name: "IX_EvaluateTarget_ObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "EndDay",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "IsUserObjectives",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "StartDay",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "KeyResults");

            migrationBuilder.DropColumn(
                name: "ObjectivesId",
                table: "EvaluateTarget");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "Quarter",
                table: "Objectives",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Objectives",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentObjectivesId",
                table: "EvaluateTarget",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserObjectivesId",
                table: "EvaluateTarget",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "DepartmentObjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DepartmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ObjectivesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Modifiedby = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentObjectives_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentObjectives_Objectives_ObjectivesId",
                        column: x => x.ObjectivesId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sidequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    KeyResultsId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Modifiedby = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sidequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sidequests_KeyResults_KeyResultsId",
                        column: x => x.KeyResultsId,
                        principalTable: "KeyResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserObjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ApplicationUserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjectivesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Modifiedby = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserObjectives_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserObjectives_Objectives_ObjectivesId",
                        column: x => x.ObjectivesId,
                        principalTable: "Objectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTarget_DepartmentObjectivesId",
                table: "EvaluateTarget",
                column: "DepartmentObjectivesId");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluateTarget_UserObjectivesId",
                table: "EvaluateTarget",
                column: "UserObjectivesId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentObjectives_DepartmentId",
                table: "DepartmentObjectives",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentObjectives_ObjectivesId",
                table: "DepartmentObjectives",
                column: "ObjectivesId");

            migrationBuilder.CreateIndex(
                name: "IX_Sidequests_KeyResultsId",
                table: "Sidequests",
                column: "KeyResultsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserObjectives_ApplicationUserId",
                table: "UserObjectives",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserObjectives_ObjectivesId",
                table: "UserObjectives",
                column: "ObjectivesId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTarget_DepartmentObjectives_DepartmentObjectivesId",
                table: "EvaluateTarget",
                column: "DepartmentObjectivesId",
                principalTable: "DepartmentObjectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluateTarget_UserObjectives_UserObjectivesId",
                table: "EvaluateTarget",
                column: "UserObjectivesId",
                principalTable: "UserObjectives",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
