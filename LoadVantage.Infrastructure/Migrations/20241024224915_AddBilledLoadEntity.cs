using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBilledLoadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BilledLoads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadId = table.Column<int>(type: "int", nullable: false),
                    BookedLoadId = table.Column<int>(type: "int", nullable: false),
                    BilledAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BilledDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BilledLoads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BilledLoads_BookedLoads_BookedLoadId",
                        column: x => x.BookedLoadId,
                        principalTable: "BookedLoads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_BilledLoads_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BilledLoads_BookedLoadId",
                table: "BilledLoads",
                column: "BookedLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_BilledLoads_LoadId",
                table: "BilledLoads",
                column: "LoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BilledLoads");
        }
    }
}
