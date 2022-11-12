using Microsoft.AspNetCore.Authentication.Cookies;
using MyRazorPage.HubServer;
using MyRazorPage.Models;

var builder = WebApplication.CreateBuilder(args);
// Bổ sung service làm việc với các Pages vào container Kestrel
builder.Services.AddRazorPages();
builder.Services.AddDbContext<PRN221_DBContext>();
builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(15));
//builder.Services.AddScoped<PRN221DBContext>();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
{
    option.LoginPath = "/account/signin";
    option.Cookie.Name = "RoleCoookie";
    option.AccessDeniedPath = "/AccessDenied";
});
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
}).AddSessionStateTempDataProvider();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("1"));
});


var app = builder.Build();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization(); 


app.UseSession();
app.MapRazorPages();
app.MapHub<SignalRServer>("/signalrServer");
app.Run();
