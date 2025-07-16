using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMPermiso
    {
        public int IdPermiso { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public string? Institucion { get; set; }

        public string Encargado { get; set; } = null!;

        public DateTime FechaVencimiento { get; set; }

        public int DiasGestion { get; set; }

        public string Criticidad { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public string EstadoPermiso { get; set; } = null!;
        public string? NombreEvidencia { get; set; }
        public string? UrlEvidencia { get; set; }
        public string? NombreEvidencia2 { get; set; }
        public string? UrlEvidencia2 { get; set; }
        public string? NombreEvidencia3 { get; set; }
        public string? UrlEvidencia3 { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
        public DateTime? FechaInicioGestion
        {
            get
            {
                if (DiasGestion > 0)
                    return FechaVencimiento.AddDays(DiasGestion*-1);
                else
                    return null;
            }
        }

        public int IdArea { get; set; }
        public string NombreArea { get; set; } = null!;
        public bool EliminarEvidencia { get; set; }
        public bool EliminarEvidencia2 { get; set; }
        public bool EliminarEvidencia3 { get; set; }
        public virtual ICollection<TbAlerta> Alertas { get; set; } = new List<TbAlerta>();
        public virtual ICollection<TbPermisoDestinatario> PermisosPorDestinatarios { get; set; } = new List<TbPermisoDestinatario>();

    }
}
