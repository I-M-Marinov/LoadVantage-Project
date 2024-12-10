using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoadVantage.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNotesPropertyFromDeliveredLoads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "DeliveredLoads");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "UsersImages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "Key used in Cloudinary to determine validity of the User Image",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "UsersImages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Url address pointing to the User Image",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UsersImages",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the User Image",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Trucks",
                type: "int",
                nullable: false,
                comment: "Truck Production Year",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TruckNumber",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Additional reference number for a truck, usually inside the company it is used in.",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Truck Model",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Truck Make",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Trucks",
                type: "bit",
                nullable: false,
                comment: "Signifies if the truck is ready to go on the road.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Trucks",
                type: "bit",
                nullable: false,
                comment: "Signifies if the truck is active or decommissioned.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier for the Driver",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Dispatcher",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a truck",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostedDate",
                table: "PostedLoads",
                type: "datetime2",
                nullable: true,
                comment: "Date and Time when the load was Posted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Posted Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "Loads",
                type: "float",
                nullable: false,
                comment: "Weight of the load in lbs",
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Loads",
                type: "int",
                nullable: false,
                comment: "Status of the load",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Loads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                comment: "Decimal amount paid for moving the load from origin to the destination.",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PickupTime",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                comment: "Date and Time when the load needs to be picked up",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "OriginState",
                table: "Loads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                comment: "The state the load is originating from",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "OriginCity",
                table: "Loads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                comment: "The city the load is originating from",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "Loads",
                type: "float",
                nullable: true,
                comment: "Distance in miles between the Origin and Destinations city and state",
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationState",
                table: "Loads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                comment: "The state the load is consigned to",
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationCity",
                table: "Loads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                comment: "The city the load is consigned to",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryTime",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                comment: "Date and Time when the load needs to be delivered at",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                comment: "Date and Time the load was created.",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "Loads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Broker",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Loads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TruckId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier for the Truck",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                comment: "License Number of the Driver",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Drivers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                comment: "Last Name of the Driver",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFired",
                table: "Drivers",
                type: "bit",
                nullable: false,
                comment: "Signifies if a driver is fired or not.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBusy",
                table: "Drivers",
                type: "bit",
                nullable: false,
                comment: "Signifies if a driver is currently on a job or not.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Drivers",
                type: "bit",
                nullable: false,
                comment: "Signifies if a driver is available to be assigned to a truck.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Drivers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                comment: "First Name of the Driver",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier for the Dispatcher",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Driver",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Driver",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Dispatcher",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveredDate",
                table: "DeliveredLoads",
                type: "datetime2",
                nullable: false,
                comment: "The date and time the load was delivered.",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Broker",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookedLoadId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier for the Booked Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Delivered Load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "ChatMessages",
                type: "datetime2",
                nullable: false,
                comment: "Time and date of the message sent",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Sender",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Receiver",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                comment: "String content sent between a Sender and a Receiver",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                comment: "Signifies if a message has been viewed or not.",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a chat message",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier for the Driver",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Dispatcher",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for the Broker",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookedDate",
                table: "BookedLoads",
                type: "datetime2",
                nullable: false,
                comment: "The date and time the load was booked.",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier for a Booked load",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "Username of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Position of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                comment: "Phone number for the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                comment: "HashedPassword for the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                comment: "Last name of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                comment: "Signifies if the user's account is activated or deactivated",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                comment: "First name of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "Email of the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "The name of the company employing the user",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "UsersImages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Key used in Cloudinary to determine validity of the User Image");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "UsersImages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Url address pointing to the User Image");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UsersImages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the User Image");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Trucks",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Truck Production Year");

            migrationBuilder.AlterColumn<string>(
                name: "TruckNumber",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Additional reference number for a truck, usually inside the company it is used in.");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Truck Model");

            migrationBuilder.AlterColumn<string>(
                name: "Make",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Truck Make");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Trucks",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if the truck is ready to go on the road.");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Trucks",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if the truck is active or decommissioned.");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier for the Driver");

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Dispatcher");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Trucks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a truck");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PostedDate",
                table: "PostedLoads",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and Time when the load was Posted");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Load");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "PostedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Posted Load");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "Loads",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldComment: "Weight of the load in lbs");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Loads",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status of the load");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Loads",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComment: "Decimal amount paid for moving the load from origin to the destination.");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PickupTime",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and Time when the load needs to be picked up");

            migrationBuilder.AlterColumn<string>(
                name: "OriginState",
                table: "Loads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldComment: "The state the load is originating from");

            migrationBuilder.AlterColumn<string>(
                name: "OriginCity",
                table: "Loads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldComment: "The city the load is originating from");

            migrationBuilder.AlterColumn<double>(
                name: "Distance",
                table: "Loads",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true,
                oldComment: "Distance in miles between the Origin and Destinations city and state");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationState",
                table: "Loads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldComment: "The state the load is consigned to");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationCity",
                table: "Loads",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldComment: "The city the load is consigned to");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveryTime",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and Time when the load needs to be delivered at");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Loads",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and Time the load was created.");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "Loads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Broker");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Loads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Load");

            migrationBuilder.AlterColumn<Guid>(
                name: "TruckId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier for the Truck");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Drivers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldComment: "License Number of the Driver");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Drivers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldComment: "Last Name of the Driver");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFired",
                table: "Drivers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if a driver is fired or not.");

            migrationBuilder.AlterColumn<bool>(
                name: "IsBusy",
                table: "Drivers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if a driver is currently on a job or not.");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAvailable",
                table: "Drivers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if a driver is available to be assigned to a truck.");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Drivers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldComment: "First Name of the Driver");

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier for the Dispatcher");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "Drivers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Driver");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Load");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Driver");

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Dispatcher");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeliveredDate",
                table: "DeliveredLoads",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "The date and time the load was delivered.");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Broker");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookedLoadId",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier for the Booked Load");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "DeliveredLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Delivered Load");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "DeliveredLoads",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "ChatMessages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Time and date of the message sent");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Sender");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Receiver");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ChatMessages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldComment: "String content sent between a Sender and a Receiver");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "ChatMessages",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if a message has been viewed or not.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a chat message");

            migrationBuilder.AlterColumn<Guid>(
                name: "LoadId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the load");

            migrationBuilder.AlterColumn<Guid>(
                name: "DriverId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier for the Driver");

            migrationBuilder.AlterColumn<Guid>(
                name: "DispatcherId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Dispatcher");

            migrationBuilder.AlterColumn<Guid>(
                name: "BrokerId",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for the Broker");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BookedDate",
                table: "BookedLoads",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "The date and time the load was booked.");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BookedLoads",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier for a Booked load");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "Username of the user");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Position of the user");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true,
                oldComment: "Phone number for the user");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "HashedPassword for the user");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldComment: "Last name of the user");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Signifies if the user's account is activated or deactivated");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(25)",
                oldMaxLength: 25,
                oldComment: "First name of the user");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldComment: "Email of the user");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "The name of the company employing the user");
        }
    }
}
