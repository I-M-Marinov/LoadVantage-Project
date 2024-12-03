using System.Threading.RateLimiting;
using LoadVantage.Areas.Admin.Contracts;
using LoadVantage.Areas.Admin.Services;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Core.Hubs;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using Microsoft.EntityFrameworkCore;
using static LoadVantage.Infrastructure.Data.SeedData.SeedData;

var builder = WebApplication.CreateBuilder(args);

// =============== Rate limiting middleware ============== // 

builder.Services.AddRateLimiter(_ => _
	.AddFixedWindowLimiter(policyName: "fixed", options =>
	{
		options.PermitLimit = 4;
		options.Window = TimeSpan.FromSeconds(12);
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = 3;
	}));

if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddUserSecrets<Program>();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<LoadVantageDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

builder.Services.AddIdentity<BaseUser, Role>(options =>
	{
		options.Password.RequireDigit = true;
		options.SignIn.RequireConfirmedAccount = false;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Lockout.MaxFailedAccessAttempts = 5;
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
		options.User.RequireUniqueEmail = true; // Unique email address 
	})
	.AddEntityFrameworkStores<LoadVantageDbContext>() 
	.AddDefaultTokenProviders();

// =============== CORS ============== // 

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
		builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
});

// =============== Register Utility services ============== // 

builder.Services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();				// Add the Sanitizer Service 

// =================================================== // 

// =============== Register USER services ============== // 

builder.Services.AddScoped<IImageService, ImageService>();									// Add the Image Service 
builder.Services.AddScoped<IProfileService, ProfileService>();                              // Add the Profile Service 
builder.Services.AddScoped<IProfileHelperService, ProfileHelperService>();                  // Add the Profile Helper Service 
builder.Services.AddScoped<IUserService, UserService>();                                    // Add the User Service 
builder.Services.AddHttpClient<ICountryStateCityService, CountryStateCityService>();		// Add CountryStateCity Service 
builder.Services.AddScoped<IGeocodeService, GeocodeService>();								// Add the Geocode Retrieval Service 
builder.Services.AddScoped<IDistanceCalculatorService, DistanceCalculatorService>();		// Add the Distance Calculator Service
builder.Services.AddScoped<ILoadStatusService, LoadStatusService>();						// Add the Load Status Service 
builder.Services.AddScoped<ILoadHelperService, LoadHelperService>();                        // Add the Load Helper Service 
builder.Services.AddScoped<ILoadBoardService, LoadBoardService>();							// Add the LoadBoard Service 
builder.Services.AddScoped<IChatService, ChatService>();									// Add the Chat Service 
builder.Services.AddScoped<ITruckService, TruckService>();									// Add the Truck Service 
builder.Services.AddScoped<IDriverService, DriverService>();								// Add the Driver Service 
builder.Services.AddSignalR();                                                              // Add SignalR

// =================================================== // 

// =============== Register ADMIN services ============== // 

builder.Services.AddScoped<IAdminProfileService, AdminProfileService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();
builder.Services.AddScoped<IAdminLoadBoardService, AdminLoadBoardService>();
builder.Services.AddScoped<IAdminLoadStatusService, AdminLoadStatusService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IAdminChatService, AdminChatService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();


// =================================================== // 

builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();

    });

builder.Services.AddRazorPages();

// =============== Cookies middleware ============== // 

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.LogoutPath = "/Home/Index";
	options.ExpireTimeSpan = TimeSpan.FromDays(1);
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.Strict;
	options.SlidingExpiration = false;

});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

	var services = scope.ServiceProvider;
	var userManager = services.GetRequiredService<UserManager<BaseUser>>();
	var configuration = services.GetRequiredService<IConfiguration>();


	await InitializeRoles(services);							// Seed the roles
	await SeedDefaultUserImage(userManager, services);			// Seed the Default User Image
	await SeedAdminUser(services, configuration);				// Seed the Administrator 
	await SeedDispatchers(services, configuration);				// Seed the Dispatchers
	await SeedBrokers(services, configuration);					// Seed the Brokers
	await SeedLoads(userManager, services);						// Seed the Created loads ( 20 random loads per each Broker )
	await SeedTrucks(userManager, services);					// Seed the Trucks ( 5 random trucks per each Dispatcher ) 
	await SeedDrivers(userManager, services);                   // Seed the Drivers ( 5 random drivers per each Dispatcher ) 

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
    builder.Configuration.AddUserSecrets<Program>();
    app.UseDeveloperExceptionPage();  // For development errors

}
else
{
	app.UseExceptionHandler("/Error/Error");  // General error handler
	app.UseStatusCodePagesWithReExecute("/Error/{0}");  // Handles status code errors (404, 500, etc.)
	app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "profile",
        pattern: "Profile",
        defaults: new { controller = "Profile", action = "Profile" });

	endpoints.MapControllerRoute(
		name: "loadboard",
		pattern: "LoadBoard/{action=LoadBoard}/{id?}",
		defaults: new { controller = "LoadBoard", action = "LoadBoard" });

	endpoints.MapControllerRoute(
	    name: "load",
	    pattern: "Load/{action}/{id?}",
	    defaults: new { controller = "Load", action = "LoadDetails" });

	endpoints.MapControllerRoute(
		name: "truck",
		pattern: "Truck/{action}/{id?}",
		defaults: new { controller = "Truck", action = "ShowTrucks" });

	endpoints.MapControllerRoute(
		name: "driver",
		pattern: "Driver/{action}/{id?}",
		defaults: new { controller = "Driver", action = "ShowDrivers" });

	endpoints.MapAreaControllerRoute(
		name: "Admin_default",
		areaName: "Admin",
		pattern: "Admin/{controller}/{action}/{id?}",
		defaults: new { area = "Admin", controller = "Admin", action = "AdminProfile" }
	);

	endpoints.MapAreaControllerRoute(
		name: "Admin_LoadBoard",
		areaName: "Admin",
		pattern: "Admin/{controller=LoadBoardManagement}/{action=Index}/{id?}",
		defaults: new { area = "Admin" }
	);

	endpoints.MapAreaControllerRoute(
		name: "AdminChat",
		areaName: "Admin",
		pattern: "Admin/{controller=AdminChat}/{action=Index}/{id?}",
		defaults: new { area = "Admin" }
	);

	endpoints.MapAreaControllerRoute(
		name: "Statistics",
		areaName: "Admin",
		pattern: "Admin/{controller=Statistics}/{action=Index}/{id?}",
		defaults: new { area = "Admin" }
	);

	endpoints.MapHub<LoadHub>("/loadHub");
	endpoints.MapHub<ChatHub>("/chatHub");

	// Default route mapping
	endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
#pragma warning restore ASP0014

//app.MapRazorPages();

app.Run();
