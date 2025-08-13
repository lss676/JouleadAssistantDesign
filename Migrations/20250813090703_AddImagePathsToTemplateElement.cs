using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JouleadAssistantDesign.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathsToTemplateElement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePaths",
                table: "TemplateElements",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePaths",
                table: "TemplateElements");
        }
    }
}
