using Encadenamiento.WebApp.Utilidades.Automapper;
using Control;
using Encadenamiento.WebApp.Utilidades.Extensiones;  //Aqui mismo esta la configuracion segura de variables de entorno
using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Agrega variables de entorno a la configuración
builder.Configuration.AddEnvironmentVariables();

// Agregá servicios
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(200);
    });

// Inyección personalizada
builder.Services.InyectarDependencia(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Lectura de secretos desde entorno
var cadenaConexion = builder.Configuration.GetConnectionString("Cadena_Conexion");
var googleApiKey = builder.Configuration["GoogleMaps:ApiKey"];

// Inyectar como servicio singleton
builder.Services.AddSingleton(new ConfiguracionSegura
{
    CadenaConexion = cadenaConexion,
    GoogleMapsApiKey = googleApiKey
});

var app = builder.Build();

// Inicializar Rotativa
RotativaConfiguration.Setup("wwwroot");

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
