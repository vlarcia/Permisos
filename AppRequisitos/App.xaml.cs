using AppRequisitos.Pages;
using AppRequisitos.Services;
using AppRequisitos.Interfaces;
using Microsoft.Maui.Storage;

namespace AppRequisitos
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Obtener el Android ID de manera segura
            var deviceInfoService = serviceProvider.GetService<IDeviceInfoService>();
            if (deviceInfoService == null)
            {
                throw new Exception("No se pudo obtener el servicio IDeviceInfoService. Verifica su implementación.");
            }

            var androidId = deviceInfoService.GetAndroidId();
            if (string.IsNullOrEmpty(androidId))
            {
                throw new Exception("No se pudo obtener el Android ID del dispositivo.");
            }

            // Conectar a la base de datos o hacer la verificación
            var deviceAuthService = new DeviceAuthService("Server=192.168.1.8;Initial Catalog=db_cumplimiento;User Id=sa;Password=Laurean0");
            if (!deviceAuthService.IsDeviceAuthorized(androidId))
            {
                // Mostrar la página de dispositivo no autorizado
                MainPage = new DeviceAuthPage(androidId);
            }
            else
            {
                // Si el dispositivo está autorizado, proceder con el login
                var logueado = Preferences.Get("logueado", string.Empty);
                if (string.IsNullOrEmpty(logueado))
                {
                    MainPage = new LoginPage(); // Si decides usar login
                }
                else
                {
                    MainPage = new AppShell();
                }
            }
        }
    }
}
