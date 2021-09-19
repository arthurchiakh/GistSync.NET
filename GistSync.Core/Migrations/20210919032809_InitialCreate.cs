using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GistSync.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncTasks",
                columns: table => new
                {
                    GistId = table.Column<string>(type: "TEXT", nullable: false),
                    SyncStrategyType = table.Column<int>(type: "INTEGER", nullable: false),
                    GistUpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GistFileName = table.Column<string>(type: "TEXT", nullable: false),
                    MappedLocalFilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileChecksum = table.Column<string>(type: "TEXT", nullable: false),
                    GitHubPersonalAccessToken = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncTasks", x => x.GistId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncTasks");
        }
    }
}
