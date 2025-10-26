using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Helpers;
using ProjectBuySmartPhone.Models.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add MyDbContext to Dependency Ịnection
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppleStore")));
//add jwthelper to dependency injection
builder.Services.AddScoped<JwtHelper>();
var app = builder.Build();
Console.WriteLine("JWT Secret Key: " + ProjectBuySmartPhone.Helpers.JwtHelper.GenerateJwtKey());

//Initlizer Data
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    DbInitializer.Initialize(services);
//}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=ScreenAdmin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
