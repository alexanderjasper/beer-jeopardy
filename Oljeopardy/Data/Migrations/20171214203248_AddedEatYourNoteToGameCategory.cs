using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oljeopardy.Data.Migrations
{
    public partial class AddedEatYourNoteToGameCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EatYourNote100",
                table: "GameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatYourNote200",
                table: "GameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatYourNote300",
                table: "GameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatYourNote400",
                table: "GameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatYourNote500",
                table: "GameCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EatYourNote100",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "EatYourNote200",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "EatYourNote300",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "EatYourNote400",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "EatYourNote500",
                table: "GameCategories");
        }
    }
}
