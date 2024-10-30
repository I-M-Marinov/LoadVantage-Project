using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LoadVantage.Infrastructure.Data.SeedData.SeedData;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<LoadVantageDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, Role>(options =>
	{
		options.Password.RequireDigit = true;
		options.SignIn.RequireConfirmedAccount = false;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Lockout.MaxFailedAccessAttempts = 5;
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	})
	.AddEntityFrameworkStores<LoadVantageDbContext>() 
	.AddDefaultTokenProviders();


builder.Services.AddScoped<IGeocodeService, GeocodeService>(); // Add the Geocode Retrieval Service 
builder.Services.AddScoped<IDistanceCalculatorService, DistanceCalculatorService>(); // Add the Distance Calculator Service 
builder.Services.AddScoped<ILoadStatusService, LoadStatusService>(); // Add the Load Status Service 
builder.Services.AddScoped<IDispatcherService, DispatcherService>(); // Add the Dispatcher Service 
builder.Services.AddScoped<IDispatcherLoadBoardService, DispatcherLoadBoardService>(); // Add the Dispatcher's LoadBoard Service 



builder.Services.AddControllersWithViews()
	.AddMvcOptions(options =>
	{
		options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();

	});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.LogoutPath = "/Home/Index";
	options.ExpireTimeSpan = TimeSpan.FromDays(2);
});

var app = builder.Build();

// Seed the Roles
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	await InitializeRoles(services);
}
// Seed the Administrator 
await SeedAdminUser(app.Services, builder.Configuration);

// Seed the Dispatchers
await SeedDispatchers(app.Services, builder.Configuration);

// Seed the Brokers
await SeedBrokers(app.Services, builder.Configuration);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapAreaControllerRoute(
//	name: "admin",
//	areaName: "Admin",
//	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapAreaControllerRoute(
//	name: "dispatcher",
//	areaName: "Dispatcher",
//	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapAreaControllerRoute(
//	name: "broker",
//	areaName: "Broker",
//	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Default route for controllers without area
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapRazorPages();

app.Run();
