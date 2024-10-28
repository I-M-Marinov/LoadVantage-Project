using LoadVantage.Areas.Dispatcher.Contracts;
using LoadVantage.Areas.Dispatcher.Services;
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

builder.Services.AddScoped<IDispatcherService, DispatcherService>(); // Add the Dispatcher Service 


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

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapRazorPages();

app.Run();
