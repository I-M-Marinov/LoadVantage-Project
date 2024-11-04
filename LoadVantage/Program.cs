using LoadVantage.Areas.Broker.Contracts;
using LoadVantage.Areas.Broker.Services;
using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Identity;
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

builder.Services.AddSession();

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

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddHttpClient<ICountryStateCityService, CountryStateCityService>(); // Add CountryStateCity Service 
builder.Services.AddScoped<IGeocodeService, GeocodeService>(); // Add the Geocode Retrieval Service 
builder.Services.AddScoped<IDistanceCalculatorService, DistanceCalculatorService>(); // Add the Distance Calculator Service
builder.Services.AddScoped<ILoadStatusService, LoadStatusService>(); // Add the Load Status Service 
builder.Services.AddScoped<IDispatcherService, DispatcherService>(); // Add the Dispatcher Service 
builder.Services.AddScoped<IBrokerService, BrokerService>(); // Add the Broker Service 
builder.Services.AddScoped<IDispatcherLoadBoardService, DispatcherLoadBoardService>(); // Add the Dispatcher's LoadBoard Service 
builder.Services.AddScoped<IBrokerLoadBoardService, BrokerLoadBoardService>(); // Add the Broker's LoadBoard Service 



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

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var configuration = services.GetRequiredService<IConfiguration>();

    await InitializeRoles(services); // Seed the roles
    await SeedAdminUser(services, configuration); // Seed the Administrator 
    await SeedDispatchers(services, configuration); // Seed the Dispatchers
    await SeedBrokers(services, configuration); // Seed the Brokers
    await SeedCreatedLoads(services, configuration, userManager); // Seed the Created loads ( 3 random loads each per broker )
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
    builder.Configuration.AddUserSecrets<Program>(); 
}
else
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
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
