using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oljeopardy.Data.Migrations
{
    public partial class FixedForeignKeyProblems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameCategories_Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_AspNetUsers_LatestChooserId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_LatestChooserId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LatestChooserId",
                table: "Games");

            migrationBuilder.AlterColumn<Guid>(
                name: "SelectedGameCategoryId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<string>(
                name: "LatestCategoryChooserId",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_SelectedGameCategoryId",
                table: "Games",
                column: "SelectedGameCategoryId",
                unique: true,
                filter: "[SelectedGameCategoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameCategories_SelectedGameCategoryId",
                table: "Games",
                column: "SelectedGameCategoryId",
                principalTable: "GameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameCategories_SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "LatestCategoryChooserId",
                table: "Games");

            migrationBuilder.AlterColumn<Guid>(
                name: "SelectedGameCategoryId",
                table: "Games",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LatestChooserId",
                table: "Games",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_LatestChooserId",
                table: "Games",
                column: "LatestChooserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GameCategories_Id",
                table: "Games",
                column: "Id",
                principalTable: "GameCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_AspNetUsers_LatestChooserId",
                table: "Games",
                column: "LatestChooserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
