using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMPlanesTrabajo
    {
        public int IdPlan { get; set; }

        public string? Descripcion { get; set; }

        public int? IdFinca { get; set; }

        public DateTime? FechaIni { get; set; }

        public DateTime? FechaFin { get; set; }

        public string? Observaciones { get; set; }

        public decimal? Avance { get; set; }

        public string? Estado { get; set; }
        public string? NombreFinca { get; set; }
        public int? CodFinca { get; set; }
        public string? Proveedor { get; set; }
        public string? Email { get; set; }


        public ICollection<VMActividad> Actividades { get; set; } = new List<VMActividad>();

    }
}
