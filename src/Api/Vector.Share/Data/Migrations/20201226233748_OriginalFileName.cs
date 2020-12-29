using Microsoft.EntityFrameworkCore.Migrations;

namespace Vector.Share.Data.Migrations
{
    public partial class OriginalFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFilename",
                table: "UploadedFiles",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFilename",
                table: "UploadedFiles");
        }
    }
}
