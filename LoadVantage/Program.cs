using System.Threading.RateLimiting;
using LoadVantage.Core.Contracts;
using LoadVantage.Core.Services;
using LoadVantage.Hubs;
using LoadVantage.Infrastructure.Data;
using LoadVantage.Infrastructure.Data.Contracts;
using LoadVantage.Infrastructure.Data.Models;
using LoadVantage.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.AddIdentity<User, Role>(options =>
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


builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
		builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader());
});


builder.Services.AddScoped<IImageService, ImageService>();									// Add the Image Service 
builder.Services.AddScoped<IProfileService, ProfileService>();                              // Add the Profile Service 
builder.Services.AddScoped<IUserService, UserService>();                                    // Add the User Service 
builder.Services.AddHttpClient<ICountryStateCityService, CountryStateCityService>();		// Add CountryStateCity Service 
builder.Services.AddScoped<IGeocodeService, GeocodeService>();								// Add the Geocode Retrieval Service 
builder.Services.AddScoped<IDistanceCalculatorService, DistanceCalculatorService>();		// Add the Distance Calculator Service
builder.Services.AddScoped<ILoadStatusService, LoadStatusService>();						// Add the Load Status Service 
builder.Services.AddScoped<ILoadBoardService, LoadBoardService>();							// Add the LoadBoard Service 
builder.Services.AddSignalR();																// Add SignalR



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
    var userManager = services.GetRequiredService<UserManager<User>>();
    var configuration = services.GetRequiredService<IConfiguration>();
    

    await InitializeRoles(services); // Seed the roles
    await SeedAdminUser(services, configuration); // Seed the Administrator 
    await SeedDispatchers(services, configuration); // Seed the Dispatchers
    await SeedBrokers(services, configuration); // Seed the Brokers
    await SeedDefaultPictures(userManager, services); // Seed the Default Images for all Users
    await SeedCreatedLoads(userManager, services); // Seed the Created loads ( 6 random loads each per broker )
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

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "profile",
        pattern: "Profile",
        defaults: new { controller = "Profile", action = "Profile" });

    endpoints.MapControllerRoute(
	    name: "loadboard",
	    pattern: "LoadBoard",
	    defaults: new { controller = "LoadBoard", action = "LoadBoard" });

	endpoints.MapControllerRoute(
	    name: "load",
	    pattern: "Load/{action}/{id?}",
	    defaults: new { controller = "Load", action = "LoadDetails" });

	endpoints.MapHub<LoadHub>("/loadHub");

	// Default route mapping
	endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
#pragma warning restore ASP0014

//app.MapRazorPages();

app.Run();
