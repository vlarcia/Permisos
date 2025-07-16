using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMAlerta
    {
        public int IdAlerta { get; set; }

        public int IdPermiso { get; set; }

        public int IdDestinatario { get; set; }

        public DateTime FechaEnvio { get; set; }

        public string MedioEnvio { get; set; } = null!;

        public string? Resultado { get; set; }

        public string? Mensaje { get; set; }
        public string? Permiso { get; set; }
        public string? Destinatario { get; set; }


    }
}
