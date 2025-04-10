using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MakeupReviewApp.Models;
using MakeupReviewApp.Repositories;
using MakeupReviewApp.Services;
using MakeupReviewApp.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Ensure it listens on 0.0.0.0:8080 (for Render)
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// ✅ Register MySQL using PlanetScale (NO SQL Server version here)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// 🔄 Add repositories/services
builder.Services.AddSingleton<MockReviewRepository>();
builder.Services.AddSingleton<MockProductRepository>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<ReviewService>();

// ✅ Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IPasswordValidator<ApplicationUser>, CustomPasswordValidator>();

// 🔒 Auth settings
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
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔧 Use Developer Exception Page in all environments (you can restrict it to Dev later)
app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Basic MVC routing
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

// ✅ Seed roles and users
try
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var user = await userManager.FindByEmailAsync("alesstongwen@gmail.com");
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = "alesstongwen@gmail.com",
                Email = "alesstongwen@gmail.com",
                FullName = "Aless",
                JoinDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "123456");
            if (!result.Succeeded)
            {
                Console.WriteLine("❌ Failed to create user:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($" - {error.Description}");
                }
            }
        }

        if (!await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("🔥 Error during role/user seeding:");
    Console.WriteLine(ex.ToString());
}


app.Run();
