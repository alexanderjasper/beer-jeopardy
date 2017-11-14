using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oljeopardy.Data.Migrations
{
    public partial class CorrectedCategoryForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GameCategories_SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SelectedGameCategoryId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Won100UserId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won200UserId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won300UserId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won400UserId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won500UserId",
                table: "GameCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedAnswerQuestionId",
                table: "Games",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Won100ParticipantId",
                table: "GameCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won200ParticipantId",
                table: "GameCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won300ParticipantId",
                table: "GameCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won400ParticipantId",
                table: "GameCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won500ParticipantId",
                table: "GameCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories",
                column: "GameId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameCategories_Games_GameId",
                table: "GameCategories",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameCategories_Games_GameId",
                table: "GameCategories");

            migrationBuilder.DropIndex(
                name: "IX_GameCategories_GameId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "SelectedAnswerQuestionId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Won100ParticipantId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won200ParticipantId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won300ParticipantId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won400ParticipantId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "Won500ParticipantId",
                table: "GameCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "SelectedGameCategoryId",
                table: "Games",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Won100UserId",
                table: "GameCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won200UserId",
                table: "GameCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won300UserId",
                table: "GameCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won400UserId",
                table: "GameCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Won500UserId",
                table: "GameCategories",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
