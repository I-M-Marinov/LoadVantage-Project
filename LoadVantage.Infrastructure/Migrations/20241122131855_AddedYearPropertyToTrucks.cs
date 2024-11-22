using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedYearPropertyToTrucks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Trucks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "Trucks");
        }
    }
}
