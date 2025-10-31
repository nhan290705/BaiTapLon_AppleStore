using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Configuration;
using ProjectBuySmartPhone.Helpers;
using ProjectBuySmartPhone.Middleware;
using ProjectBuySmartPhone.Models.Infrastructure;
using ProjectBuySmartPhone.Responsitory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<JwtAuthFilter>();
});

// Add MyDbContext to Dependency Ịnection
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppleStore")));
builder.Services.AddScoped<IStatusResponsitory, StatusResponsitory>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//add jwthelper to dependency injection
builder.Services.AddScoped<JwtHelper>();

//Add Authentication
JwtConfig.BuildJwtConfig(builder);
var app = builder.Build();
//Console.WriteLine("JWT Secret Key: " + ProjectBuySmartPhone.Helpers.JwtHelper.GenerateJwtKey());




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/") // chỉ check trang gốc
    {
        var accessToken = context.Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(accessToken))
        {
            // Redirect sang HomePage
            context.Response.Redirect("/HomePage/HomePage");
            return;
        }
    }

    await next();
});

app.UseRouting();
//dki middleware token

app.UseStaticFiles();
app.UseMiddleware<RefreshJwtMiddleware>();
app.UseAuthentication(); //Chien

app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=ScreenAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
