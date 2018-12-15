using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedditFlairs.Core.Migrations
{
    public partial class AddFlairsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FlairsUpdated",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlairsUpdated",
                table: "Users");
        }
    }
}
