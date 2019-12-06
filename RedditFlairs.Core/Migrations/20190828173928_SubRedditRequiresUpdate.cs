using Microsoft.EntityFrameworkCore.Migrations;

namespace RedditFlairs.Core.Migrations
{
    public partial class SubRedditRequiresUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresUpdate",
                table: "SubReddits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresUpdate",
                table: "SubReddits");
        }
    }
}
