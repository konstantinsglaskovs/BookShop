using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateImageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "UserProfiles",
                newName: "ProfileImageStorageKey");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "BookImages",
                newName: "StorageKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageStorageKey",
                table: "UserProfiles",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "BookImages",
                newName: "ImageUrl");
        }
    }
}
