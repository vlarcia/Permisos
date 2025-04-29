using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMRevisiones
    {
        public int IdRevision { get; set; }
        public int IdFinca { get; set; }
        public string? NombreFinca { get; set; }
        public int IdRequisito { get; set; }
        public DateTime Fecha { get; set; }
        public string? Estado { get; set; }
        public string? Observaciones { get; set; }
        public string? Comentarios { get; set; }
        public string? Tipo { get; set; }
        public decimal? Cumplimiento { get; set; }        
        public string? Requisito { get; set; }
        public string? UrlFoto1 { get; set; }
        public string? UrlFoto2 { get; set; }
        public string? Proveedor { get; set; }
        public string? Ambito { get; set; }
        public int? CodFinca { get; set; }
        public string? Email { get; set; }
        public int? Grupo { get; set; }

    }
}
