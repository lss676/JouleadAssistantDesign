using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JouleadAssistantDesign.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToElementConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "ElementConfigs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "ElementConfigs");
        }
    }
}
