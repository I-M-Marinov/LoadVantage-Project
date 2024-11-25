using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNavigationalPropertyBetweenBookedAndDeliveredLoads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookedLoadId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveredLoads_BookedLoadId",
                table: "DeliveredLoads",
                column: "BookedLoadId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveredLoads_BookedLoads_BookedLoadId",
                table: "DeliveredLoads",
                column: "BookedLoadId",
                principalTable: "BookedLoads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveredLoads_BookedLoads_BookedLoadId",
                table: "DeliveredLoads");

            migrationBuilder.DropIndex(
                name: "IX_DeliveredLoads_BookedLoadId",
                table: "DeliveredLoads");

            migrationBuilder.DropColumn(
                name: "BookedLoadId",
                table: "DeliveredLoads");
        }
    }
}
