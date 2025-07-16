using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMActividad
    {
        public int IdActividad { get; set; }

        public int? IdPlan { get; set; }

        public int? IdRequisito { get; set; }

        public string? Descripcion { get; set; }

        public string? Tipo { get; set; }

        public DateTime? FechaIni { get; set; }

        public DateTime? FechaFin { get; set; }

        public string? Responsable { get; set; }

        public string? Recursos { get; set; }

        public decimal? Avances { get; set; }

        public decimal? Avanceanterior { get; set; }

        public string? Estado { get; set; }

        public string? Comentarios { get; set; }

        public DateTime? FechaUltimarevision { get; set; }

        public int? IdFinca { get; set; }  // Relación con la finca
        public string? NombreFinca { get; set; }
        public string? NombrePlan { get; set; }
        public DateTime? FechaPlanIni { get; set; }

    }
}
