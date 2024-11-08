using Encadenamiento.WebApp.Utilidades.Automapper;
using Control;
using Encadenamiento.WebApp.Utilidades.Extensiones;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rotativa.AspNetCore;  //para PDF

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan=TimeSpan.FromMinutes(30);
    });

builder.Services.InyectarDependencia(builder.Configuration);
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


var app = builder.Build();

// Inicializar Rotativa con la ruta de los binarios de wkhtmltopdf
RotativaConfiguration.Setup("wwwroot");  // Establece la ruta de los binarios

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
