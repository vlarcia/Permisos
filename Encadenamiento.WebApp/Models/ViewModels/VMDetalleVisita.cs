using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMDetalleVisita
    {
        public int IdReg { get; set; }

        public int IdVisita { get; set; }

        public int IdActividad { get; set; }

        public DateTime? Fecha { get; set; }

        public decimal? Avances { get; set; }

        public decimal? Avanceanterior { get; set; }

        public string? Estado { get; set; }

        public string? Estadoanterior { get; set; }

        public string? Comentarios { get; set; }

        public string? Observaciones { get; set; }

        public string? Urlfoto1 { get; set; }

        public string? Nombrefoto1 { get; set; }

        public int? IdFinca { get; set; }
        public string? DescripcionActividad { get; set; }
        public string? Tipo { get; set; }
        public DateTime? FechaInicia { get; set; }
        public DateTime? FechaFinaliza { get; set; }
        public DateTime? FechaUltimarevision { get; set; }       
    }
}

