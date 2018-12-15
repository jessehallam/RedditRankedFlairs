using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RedditFlairs.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankWeights",
                columns: table => new
                {
                    RankName = table.Column<string>(nullable: false),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankWeights", x => x.RankName);
                });

            migrationBuilder.CreateTable(
                name: "SubReddits",
                columns: table => new
                {
                    Name = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    CssPattern = table.Column<string>(unicode: false, maxLength: 20, nullable: false, defaultValue: ""),
                    FlairPattern = table.Column<string>(unicode: false, maxLength: 20, nullable: false, defaultValue: ""),
                    QueueTypes = table.Column<string>(unicode: false, maxLength: 200, nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubReddits", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "SummonerValidations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttemptedAt = table.Column<DateTimeOffset>(nullable: true),
                    Attempts = table.Column<int>(nullable: false),
                    Code = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummonerValidations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TierWeights",
                columns: table => new
                {
                    TierName = table.Column<string>(nullable: false),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TierWeights", x => x.TierName);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(unicode: false, maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Summoners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AccountId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    PUUID = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    RankUpdatedAt = table.Column<DateTimeOffset>(nullable: true),
                    Region = table.Column<string>(unicode: false, maxLength: 8, nullable: false),
                    SummonerId = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    SummonerName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summoners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Summoners_SummonerValidations_Id",
                        column: x => x.Id,
                        principalTable: "SummonerValidations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Summoners_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFlairs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CssText = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    NeedToSend = table.Column<bool>(nullable: false),
                    Text = table.Column<string>(unicode: false, maxLength: 50, nullable: false, defaultValue: ""),
                    SubRedditName = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFlairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFlairs_SubReddits_SubRedditName",
                        column: x => x.SubRedditName,
                        principalTable: "SubReddits",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFlairs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LeaguePositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    QueueType = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    Rank = table.Column<string>(unicode: false, maxLength: 20, nullable: false),
                    SummonerId = table.Column<int>(nullable: false),
                    Tier = table.Column<string>(unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaguePositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaguePositions_Summoners_SummonerId",
                        column: x => x.SummonerId,
                        principalTable: "Summoners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaguePositions_SummonerId",
                table: "LeaguePositions",
                column: "SummonerId");

            migrationBuilder.CreateIndex(
                name: "IX_Summoners_UserId",
                table: "Summoners",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlairs_SubRedditName",
                table: "UserFlairs",
                column: "SubRedditName");

            migrationBuilder.CreateIndex(
                name: "IX_UserFlairs_UserId",
                table: "UserFlairs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaguePositions");

            migrationBuilder.DropTable(
                name: "RankWeights");

            migrationBuilder.DropTable(
                name: "TierWeights");

            migrationBuilder.DropTable(
                name: "UserFlairs");

            migrationBuilder.DropTable(
                name: "Summoners");

            migrationBuilder.DropTable(
                name: "SubReddits");

            migrationBuilder.DropTable(
                name: "SummonerValidations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
