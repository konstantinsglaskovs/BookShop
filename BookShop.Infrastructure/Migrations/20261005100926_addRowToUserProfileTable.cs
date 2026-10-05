using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRowToUserProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Surname",
                table: "UserProfiles");
        }
    }
}
