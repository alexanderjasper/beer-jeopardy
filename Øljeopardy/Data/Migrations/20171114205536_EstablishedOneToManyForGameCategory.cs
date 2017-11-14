using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oljeopardy.Data.Migrations
{
    public partial class EstablishedOneToManyForGameCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedGameCategoryId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_SelectedGameCategoryId",
                table: "Games",
                column: "SelectedGameCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories",
                column: "GameId");

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

            migrationBuilder.DropIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories",
                column: "GameId",
                unique: true);
        }
    }
}
