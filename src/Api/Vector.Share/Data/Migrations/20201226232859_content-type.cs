using Microsoft.EntityFrameworkCore.Migrations;

namespace Vector.Share.Data.Migrations
{
    public partial class contenttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "UploadedFiles",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "UploadedFiles");
        }
    }
}
