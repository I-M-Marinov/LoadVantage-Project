using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatePropertiesToLoad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DestinationLatitude",
                table: "Loads",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DestinationLongitude",
                table: "Loads",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OriginLatitude",
                table: "Loads",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OriginLongitude",
                table: "Loads",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationLatitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "DestinationLongitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "OriginLatitude",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "OriginLongitude",
                table: "Loads");
        }
    }
}
