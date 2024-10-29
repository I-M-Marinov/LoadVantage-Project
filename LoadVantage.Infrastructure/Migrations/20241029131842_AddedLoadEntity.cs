using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLoadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_BilledLoads_BookedLoads_BookedLoadId",
                table: "BilledLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BilledLoads_PostedLoads_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_AspNetUsers_BrokerId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_AspNetUsers_DispatcherId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_Drivers_DriverId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_PostedLoads_LoadId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AspNetUsers_DispatcherId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_BilledLoads_BookedLoadId",
                table: "BilledLoads");

            migrationBuilder.DropIndex(
                name: "IX_BilledLoads_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropColumn(
                name: "DeliveryTime",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "DestinationCity",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "DestinationState",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "OriginCity",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "OriginState",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "PickupTime",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "PostedPrice",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "PostedLoads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookedLoads");

            migrationBuilder.DropColumn(
                name: "BookedLoadId",
                table: "BilledLoads");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BilledLoads");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "LoadId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.CreateTable(
                name: "Load",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginCity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OriginState = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    DestinationCity = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DestinationState = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BrokerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Load", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Load_AspNetUsers_BrokerId",
                        column: x => x.BrokerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostedLoads_LoadId",
                table: "PostedLoads",
                column: "LoadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BilledLoads_LoadId",
                table: "BilledLoads",
                column: "LoadId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Load_BrokerId",
                table: "Load",
                column: "BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BilledLoads_Load_LoadId",
                table: "BilledLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_AspNetUsers_BrokerId",
                table: "BookedLoads",
                column: "BrokerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_AspNetUsers_DispatcherId",
                table: "BookedLoads",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_Drivers_DriverId",
                table: "BookedLoads",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_Load_LoadId",
                table: "BookedLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PostedLoads_Load_LoadId",
                table: "PostedLoads",
                column: "LoadId",
                principalTable: "Load",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AspNetUsers_DispatcherId",
                table: "Trucks",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_BilledLoads_Load_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_AspNetUsers_BrokerId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_AspNetUsers_DispatcherId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_Drivers_DriverId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedLoads_Load_LoadId",
                table: "BookedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_PostedLoads_Load_LoadId",
                table: "PostedLoads");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_AspNetUsers_DispatcherId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "Load");

            migrationBuilder.DropIndex(
                name: "IX_PostedLoads_LoadId",
                table: "PostedLoads");

            migrationBuilder.DropIndex(
                name: "IX_BilledLoads_LoadId",
                table: "BilledLoads");

            migrationBuilder.DropColumn(
                name: "LoadId",
                table: "PostedLoads");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryTime",
                table: "PostedLoads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DestinationCity",
                table: "PostedLoads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DestinationState",
                table: "PostedLoads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "PostedLoads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OriginCity",
                table: "PostedLoads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginState",
                table: "PostedLoads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupTime",
                table: "PostedLoads",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "PostedPrice",
                table: "PostedLoads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PostedLoads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "PostedLoads",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BookedLoads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "BookedLoadId",
                table: "BilledLoads",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "BilledLoads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BilledLoads_BookedLoadId",
                table: "BilledLoads",
                column: "BookedLoadId");

            migrationBuilder.CreateIndex(
                name: "IX_BilledLoads_LoadId",
                table: "BilledLoads",
                column: "LoadId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetRoles_RoleId",
                table: "AspNetUsers",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledLoads_BookedLoads_BookedLoadId",
                table: "BilledLoads",
                column: "BookedLoadId",
                principalTable: "BookedLoads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledLoads_PostedLoads_LoadId",
                table: "BilledLoads",
                column: "LoadId",
                principalTable: "PostedLoads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_AspNetUsers_BrokerId",
                table: "BookedLoads",
                column: "BrokerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_AspNetUsers_DispatcherId",
                table: "BookedLoads",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_Drivers_DriverId",
                table: "BookedLoads",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedLoads_PostedLoads_LoadId",
                table: "BookedLoads",
                column: "LoadId",
                principalTable: "PostedLoads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_AspNetUsers_DispatcherId",
                table: "Trucks",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
