using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUsersAndUsersImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UsersImages_UserImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserImageId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersImages");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserImageId",
                table: "AspNetUsers",
                column: "UserImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UsersImages_UserImageId",
                table: "AspNetUsers",
                column: "UserImageId",
                principalTable: "UsersImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UsersImages",
                type: "uniqueidentifier",
                nullable: true);

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
    }
}
