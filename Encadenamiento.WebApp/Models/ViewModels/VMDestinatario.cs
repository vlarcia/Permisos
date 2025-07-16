using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMDestinatario
    {
        public int IdDestinatario { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Correo { get; set; }

        public string? TelefonoWhatsapp { get; set; }

        public bool RecibeAlta { get; set; }

        public bool RecibeMedia { get; set; }

        public bool RecibeBaja { get; set; }

        public bool? Activo { get; set; }

        public DateTime FechaRegistro { get; set; }
        public int IdArea { get; set; }
        public string NombreArea { get; set; } = null!;

        public virtual ICollection<VMAlerta> Alertas { get; set; } = new List<VMAlerta>();
        public virtual ICollection<VMPermisoDestinatario> PermisosPorDestinatarios { get; set; } = new List<VMPermisoDestinatario>();

    }
}
