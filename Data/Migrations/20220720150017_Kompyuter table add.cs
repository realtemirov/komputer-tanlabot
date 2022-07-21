using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bot.Data.Migrations
{
    public partial class Kompyutertableadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChoosenApps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    ProgId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    LastInteractionAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoosenApps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kompyuters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ModelName = table.Column<string>(type: "TEXT", nullable: true),
                    OS = table.Column<string>(type: "TEXT", nullable: true),
                    ScreenSize = table.Column<string>(type: "TEXT", nullable: true),
                    Processor = table.Column<string>(type: "TEXT", nullable: true),
                    RAM = table.Column<string>(type: "TEXT", nullable: true),
                    Storage = table.Column<string>(type: "TEXT", nullable: true),
                    GPU = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<string>(type: "TEXT", nullable: true),
                    Grade = table.Column<double>(type: "REAL", nullable: false),
                    LinkOfPic = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kompyuters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Progs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ProgType = table.Column<int>(type: "INTEGER", nullable: false),
                    Point = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoosenApps");

            migrationBuilder.DropTable(
                name: "Kompyuters");

            migrationBuilder.DropTable(
                name: "Progs");
        }
    }
}
