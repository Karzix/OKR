using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OKR.Models.Migrations
{
    /// <inheritdoc />
    public partial class removeTableTargetType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_TargetType_TargetTypeId",
                table: "Objectives");

            migrationBuilder.DropTable(
                name: "RefreshTokenModel");

            migrationBuilder.DropTable(
                name: "TargetType");

            migrationBuilder.DropIndex(
                name: "IX_Objectives_TargetTypeId",
                table: "Objectives");

            migrationBuilder.DropColumn(
                name: "TargetTypeId",
                table: "Objectives");

            migrationBuilder.AddColumn<int>(
                name: "TargetType",
                table: "Objectives",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetType",
                table: "Objectives");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetTypeId",
                table: "Objectives",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RefreshTokenModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifiedby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokenModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LevelApply = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Modifiedby = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objectives_TargetTypeId",
                table: "Objectives",
                column: "TargetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_TargetType_TargetTypeId",
                table: "Objectives",
                column: "TargetTypeId",
                principalTable: "TargetType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
