using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MakeupReviewApp.Models;
using MakeupReviewApp.Repositories;
using MakeupReviewApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Register repositories
builder.Services.AddSingleton<MockReviewRepository>();
builder.Services.AddSingleton<MockProductRepository>();
builder.Services.AddSingleton<MockUserRepository>();

// Register services
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<ReviewService>();

// Configure database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Logging.AddConsole();
builder.Logging.AddDebug();


// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

builder.Services.AddAuthorization();
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// Method to create roles
async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "User" };
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Method to seed users
async Task SeedUsers(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var users = new List<User>
    {
        new User { FullName = "Alice Johnson", Email = "alice@example.com", Password = "password123" },
        new User { FullName = "Brenda Smith", Email = "bob@example.com", Password = "securepass" },
        new User { FullName = "Aless", Email = "alesstongwen@gmail.com", Password = "123456" }
    };

    foreach (var user in users)
    {
        if (await userManager.FindByEmailAsync(user.Email) == null)
        {
            var identityUser = new IdentityUser { UserName = user.Email, Email = user.Email };
            await userManager.CreateAsync(identityUser, user.Password);
        }
    }
}

// Call CreateRoles and SeedUsers methods
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
    await SeedUsers(services);
}

app.Run();