using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledLoads_Load_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_Load_LoadId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_Load_AspNetUsers_BrokerId",
                table: "Load");

            migrationBuilder.DropForeignKey(
                name: "FK_PostedLoads_AspNetUsers_BrokerId",
                table: "PostedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_PostedLoads_Load_LoadId",
                table: "PostedLoads");

            migrationBuilder.DropIndex(
                name: "IX_PostedLoads_BrokerId",
                table: "PostedLoads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Load",
                table: "Load");

            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "PostedLoads");

            migrationBuilder.RenameTable(
                name: "Load",
                newName: "Loads");

            migrationBuilder.RenameIndex(
                name: "IX_Load_BrokerId",
                table: "Loads",
                newName: "IX_Loads_BrokerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Loads",
                table: "Loads",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledLoads_Loads_LoadId",
                table: "BilledLoads",
                column: "LoadId",
                principalTable: "Loads",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_Loads_LoadId",
                table: "BookedLoads",
                column: "LoadId",
                principalTable: "Loads",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_AspNetUsers_BrokerId",
                table: "Loads",
                column: "BrokerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PostedLoads_Loads_LoadId",
                table: "PostedLoads",
                column: "LoadId",
                principalTable: "Loads",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledLoads_Loads_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_Loads_LoadId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_Loads_AspNetUsers_BrokerId",
                table: "Loads");

            migrationBuilder.DropForeignKey(
                name: "FK_PostedLoads_Loads_LoadId",
                table: "PostedLoads");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Loads",
                table: "Loads");

            migrationBuilder.RenameTable(
                name: "Loads",
                newName: "Load");

            migrationBuilder.RenameIndex(
                name: "IX_Loads_BrokerId",
                table: "Load",
                newName: "IX_Load_BrokerId");

            migrationBuilder.AddColumn<Guid>(
                name: "BrokerId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Load",
                table: "Load",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PostedLoads_BrokerId",
                table: "PostedLoads",
                column: "BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledLoads_Load_LoadId",
                table: "BilledLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_Load_LoadId",
                table: "BookedLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Load_AspNetUsers_BrokerId",
                table: "Load",
                column: "BrokerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PostedLoads_AspNetUsers_BrokerId",
                table: "PostedLoads",
                column: "BrokerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostedLoads_Load_LoadId",
                table: "PostedLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
