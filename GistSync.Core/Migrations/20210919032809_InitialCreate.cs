using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GistSync.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncTasks",
                columns: table => new {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    GistId = table.Column<string>(type: "TEXT", nullable: false),
                    SyncMode = table.Column<int>(type: "INTEGER", nullable: false), 
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Directory = table.Column<string>(type: "TEXT", nullable: false),
                    GitHubPersonalAccessToken = table.Column<string>(type: "TEXT", nullable: true),
                    IsEnabled = table.Column<bool>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncTaskFiles",
                columns: table => new {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    SyncTaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileChecksum = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncTaskFiles", x => x.Id);
                    table.ForeignKey("FK_SyncTasks_SyncTaskFiles",
                        f => f.SyncTaskId,
                        "SyncTasks",
                        "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncTasks");
        }
    }
}
