using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meetings.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Meetings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Meetings");
        }
    }
}
