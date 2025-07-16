using System;
using System.IO;

namespace Permisos.ConsoleApp
{
    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log_alertas.txt");

        public static bool HuboErrores { get; private set; } = false;

        public static void Info(string mensaje)
        {
            EscribirLog("INFO", mensaje);
        }

        public static void Error(string mensaje)
        {
            HuboErrores = true;
            EscribirLog("ERROR", mensaje);
        }

        private static void EscribirLog(string tipo, string mensaje)
        {
            try
            {
                string linea = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{tipo}] {mensaje}";
                File.AppendAllLines(logFilePath, new[] { linea });
            }
            catch
            {
                // Evitar fallas en escritura de log
            }
        }

        public static string ObtenerRutaLog()
        {
            return logFilePath;
        }
    }
}
