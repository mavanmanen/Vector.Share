using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Vector.Share.Data.Migrations
{
    public partial class ScheduledDeletions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledDeletions",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "TEXT", nullable: false),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledDeletions", x => x.Identifier);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledDeletions");
        }
    }
}
