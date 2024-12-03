using Android.Provider;
using AppRequisitos.Interfaces;
using Microsoft.Maui.Controls;

namespace AppRequisitos
{
    public class DeviceInfoService : IDeviceInfoService
    {
        public string GetAndroidId()
        {
            // Usamos Android.App.Application.Context
            var context = Android.App.Application.Context;

            // Obtén el ANDROID_ID desde Settings.Secure
            var androidId = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);

            return androidId;
        }
    }
}
