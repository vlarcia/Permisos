using AppRequisitos.Pages;
using Microsoft.Extensions.Logging;
using Camera.MAUI;
using AppRequisitos.DataAccess;
using AppRequisitos.ViewModels;
using CommunityToolkit.Maui;
using AppRequisitos.Services;
using Microsoft.EntityFrameworkCore;
using AppRequisitos.Utilidades;
using AppRequisitos.Interfaces;
using System.Net;

namespace AppRequisitos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCameraView()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa-solid-900.ttf", "FaSolid");
                });

            // Registro de servicios
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            builder.Services.AddDbContext<CumplimientoDbContext>();
            builder.Services.AddDbContext<SQLServerDbContext>();
            builder.Services.AddTransient<ActividadService>();
            builder.Services.AddSingleton<FincaService>();
            builder.Services.AddSingleton<SyncService>();
            builder.Services.AddSingleton(sp =>
                new DeviceAuthService("Server=192.168.1.8;Initial Catalog=db_cumplimiento;User Id=sa;Password=Laurean0"));

            builder.Services.AddHttpClient();

            builder.Services.AddTransient<SyncPage>(sp => new SyncPage(
                sp.GetRequiredService<CumplimientoDbContext>(),
                sp.GetRequiredService<SyncService>())
            );

            builder.Services.AddTransient<FincasPage>();
            builder.Services.AddTransient<FincasVM>();
            builder.Services.AddTransient<ChecklistPage>();
            builder.Services.AddTransient<ChecklistVM>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainVM>();

            // Registro del servicio para obtener el Android ID
#if ANDROID
            builder.Services.AddSingleton<IDeviceInfoService, DeviceInfoService>();
#endif

            // Crear la base de datos SQLite si no existe
            var dbContext = new CumplimientoDbContext();
            dbContext.Database.EnsureCreated();
            dbContext.Dispose();

#if DEBUG
            builder.Logging.AddDebug();
#endif
#if ANDROID && DEBUG
            Platforms.Android.DangerousTrustProvider.Register();
#endif

            Routing.RegisterRoute(nameof(FincasPage), typeof(FincasPage));
            Routing.RegisterRoute(nameof(SyncPage), typeof(SyncPage));
            Routing.RegisterRoute(nameof(ChecklistPage), typeof(ChecklistPage));
            Routing.RegisterRoute(nameof(ChecklistDetallePage), typeof(ChecklistDetallePage));

            return builder.Build();
        }
    }
}