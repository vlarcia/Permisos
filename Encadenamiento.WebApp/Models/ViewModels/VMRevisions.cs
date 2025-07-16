using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    // Esta tabla surgió por necesidad y no se consideró en el diseño original
    // evitar confundor con el detalle de la revision, se hizo para contener
    // las fotos y control de envío de correos.
    public class VMRevisions
    {
        public int IdReg { get; set; }
        public int IdFinca { get; set; }
        public string? NombreFinca { get; set; }        
        public DateTime Fecha { get; set; }        
        public string? Observaciones { get; set; }        
        public decimal? Cumplimiento { get; set; }        
        public string? Tipo { get; set; }
        public int? SentTo { get; set; }
        public bool? Sincronizado { get; set; }
        public bool? Aplicado { get; set; }
        public string? Nombrefoto1 { get; set; }
        public string? Nombrefoto2 { get; set; }
        public string? Nombrefoto3 { get; set; }
        public string? Nombrefoto4 { get; set; }
        public string? Urlfoto1 { get; set; }
        public string? Urlfoto2 { get; set; }
        public string? Urlfoto3 { get; set; }
        public string? Urlfoto4 { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public string? Proveedor { get; set; }        
        public int? CodFinca { get; set; }
        public string? Email { get; set; }
        public int? Grupo { get; set; }
        public string? Telefono { get; set; }
        public string? MapaBase64 { get; set; }
    }
}
