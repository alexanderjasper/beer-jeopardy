using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Oljeopardy.Data.Migrations
{
    public partial class RevertedEatYourNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EatYourNotes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EatYourNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AnswerQuestionId = table.Column<Guid>(nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EatYourNotes", x => x.Id);
                });
        }
    }
}
