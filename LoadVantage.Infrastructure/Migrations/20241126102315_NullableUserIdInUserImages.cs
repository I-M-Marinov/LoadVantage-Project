using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NullableUserIdInUserImages : Migration
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

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersImages",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersImages_AspNetUsers_UserId",
                table: "UsersImages");

            migrationBuilder.DropIndex(
                name: "IX_UsersImages_UserId",
                table: "UsersImages");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UsersImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersImages_UserId",
                table: "UsersImages",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersImages_AspNetUsers_UserId",
                table: "UsersImages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
