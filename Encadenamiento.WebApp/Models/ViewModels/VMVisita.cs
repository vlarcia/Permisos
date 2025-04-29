using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMVisita
    {
        public int IdVisita { get; set; }
        public int IdFinca { get; set; } 
        public int IdPlan { get; set; }
        public string? Observaciones { get; set; }
        public int? Zafra { get; set; }
        public DateTime? Fecha { get; set; }
        public string? AndroidId { get; set; }
        public int? SentTo { get; set; }
        public string? Responsable { get; set; }
        public string? Mandador { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public string? NombreFinca { get; set; }
        public string? CodFinca { get; set; }
        public string? Proveedor { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? DescripcionPlan { get; set; }
        public string? UrlFoto1 { get; set; }
        public string? UrlFoto2 { get; set; }
        public string? UrlFoto3 { get; set; }
        public string? UrlFoto4 { get; set; }
        public string? NombreFoto1 { get; set; }        
        public string? NombreFoto2 { get; set; }        
        public string? NombreFoto3 { get; set; }
        public string? NombreFoto4 { get; set; }
        public bool? Sincronizado { get; set; }
        public bool? Aplicado { get; set; }
        public string? MapaBase64 { get; set; }
        public virtual ICollection<VMDetalleVisita> DetalleVisita { get; set; } = new List<VMDetalleVisita>();
 
    }
}
