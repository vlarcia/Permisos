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
            services.AddDbContext<CumplimientoContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Cadena_Conexion"));
            });
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IPlanesRepository, PlanesRepository>();

            services.AddScoped<IVisitaRepository, VisitaRepository>();

            services.AddScoped<ICorreoService, CorreoService>();

            services.AddScoped<IFireBaseService, FireBaseService>();

            services.AddScoped<IUtilidadesService, UtilidadesService>();

            services.AddScoped<IRolService, RolService>();

            services.AddScoped<IUsuarioService, UsuarioService>();

            services.AddScoped<IMaestroFincaService, MaestroFincaService>();

            services.AddScoped<ICheckListService, CheckListService>();

            services.AddScoped<INegocioService, NegocioService>();

            services.AddScoped<IPlanesTrabajoService, PlanesTrabajoService>();

            services.AddScoped<IDashBoardService, DashBoardService>();

            services.AddScoped<IMenuService, MenuService>();

            services.AddScoped<IRevisionService, RevisionService>();

            services.AddScoped<IVisitaService, VisitaService>();

            services.AddScoped<IParametroService, ParametroService>();

        }
    }
}
