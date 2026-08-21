using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using SistemaInventario.Data;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Data.Repositories;

QuestPDF.Settings.License =
    QuestPDF.Infrastructure.LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ConexionBD>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();
builder.Services.AddScoped<IMovimientoRepositorio, MovimientoRepositorio>();
builder.Services.AddScoped<IDashboardRepositorio, DashboardRepositorio>();
builder.Services.AddScoped<UsuarioRepositorio>();
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Login";
        opciones.AccessDeniedPath = "/Cuenta/Login";
    });
builder.Services.AddHttpClient("api", cliente =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]
        ?? "http://localhost:5012/";
    cliente.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "sin-ip",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllers();
app.Run();