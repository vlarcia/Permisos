using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Data.DBContext;
using Microsoft.EntityFrameworkCore;
using Data.Interfaces;
using Data.Implementacion;
using Business.Interfaces;
using Business.Implementacion;




namespace Control
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<PermisosContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Cadena_ConexionPermisos"));
            });
          
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
          

            services.AddScoped<ICorreoService, CorreoService>();

            services.AddScoped<IFireBaseService, FireBaseService>();

            services.AddScoped<IUtilidadesService, UtilidadesService>();

            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();           

            services.AddScoped<INegocioService, NegocioService>();
           
            services.AddScoped<IDashBoardService, DashBoardService>();

            services.AddScoped<IMenuService, MenuService>();            

            services.AddScoped<IParametroService, ParametroService>();

            services.AddScoped<IAlertaService, AlertaService>();

            services.AddScoped<IDestinatarioService, DestinatarioService>();

            services.AddScoped<IPermisoService, PermisoService>();

            services.AddScoped<IPlantillaService, PlantillaService>();

            services.AddScoped<IAreaService, AreaService>();

            services.AddHttpClient();

            services.AddScoped<GoogleMapsService>();
        }
    }
}
