using Microsoft.EntityFrameworkCore.Migrations;

namespace Vector.Share.Data.Migrations
{
    public partial class Extension : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "UploadedFiles",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extension",
                table: "UploadedFiles");
        }
    }
}
