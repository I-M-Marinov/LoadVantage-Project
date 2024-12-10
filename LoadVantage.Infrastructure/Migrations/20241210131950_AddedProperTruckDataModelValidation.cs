using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedProperTruckDataModelValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TruckNumber",
                table: "Trucks",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                comment: "Additional reference number for a truck, usually inside the company it is used in.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Additional reference number for a truck, usually inside the company it is used in.");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Trucks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Truck Model",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Truck Model");

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Trucks",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                comment: "Truck Make",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Truck Make");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TruckNumber",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Additional reference number for a truck, usually inside the company it is used in.",
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldComment: "Additional reference number for a truck, usually inside the company it is used in.");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Truck Model",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Truck Model");

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Truck Make",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldComment: "Truck Make");
        }
    }
}
