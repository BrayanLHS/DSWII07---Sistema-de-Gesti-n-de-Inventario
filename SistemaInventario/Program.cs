using SistemaInventario.Data;
using SistemaInventario.Data.Interfaces;
using SistemaInventario.Data.Repositories;

// Licencia gratuita de QuestPDF para generar los PDF (uso comunitario/educativo)
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar servicios (por interfaz, para poder cambiar la implementación sin tocar los controladores)
builder.Services.AddSingleton<ConexionBD>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();
builder.Services.AddScoped<IMovimientoRepositorio, MovimientoRepositorio>();
builder.Services.AddScoped<IDashboardRepositorio, DashboardRepositorio>();

// CORS: permite que otros clientes (o esta misma app) consuman la API libremente
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// HttpClient con el que ClienteController consume ProductosApiController
builder.Services.AddHttpClient("api", cliente =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5012/";
    cliente.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors("PermitirTodo");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Habilita las rutas de los controladores de API ([ApiController]/[Route])
app.MapControllers();

app.Run();