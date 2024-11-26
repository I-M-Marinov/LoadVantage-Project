using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationshipUserToUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersImages_AspNetUsers_UserId",
                table: "UsersImages");

            migrationBuilder.DropIndex(
                name: "IX_UsersImages_UserId",
                table: "UsersImages");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserImageId",
                table: "AspNetUsers",
                column: "UserImageId",
                unique: true,
                filter: "[UserImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UsersImages_UserImageId",
                table: "AspNetUsers",
                column: "UserImageId",
                principalTable: "UsersImages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UsersImages_UserImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserImageId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_UsersImages_UserId",
                table: "UsersImages",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersImages_AspNetUsers_UserId",
                table: "UsersImages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
