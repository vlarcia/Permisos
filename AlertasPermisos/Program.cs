using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Business.Interfaces;
using Business.Implementacion;
using Data.DBContext;
using Data.Interfaces;
using Data.Implementacion;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<PermisosContext>(options =>
            options.UseSqlServer("Server=localhost;Initial Catalog=db_Permisos;User ID=sa;Password=Laurean0;TrustServerCertificate=True"));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IPermisoService, PermisoService>();
        services.AddScoped<IAlertaService, AlertaService>();
        services.AddScoped<ICorreoService, CorreoService>();
        services.AddScoped<IPlantillaService, PlantillaService>();
        services.AddScoped<IDestinatarioService, DestinatarioService>();
        services.AddScoped<INegocioService, NegocioService>();

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
