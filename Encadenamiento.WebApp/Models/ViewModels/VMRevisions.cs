using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
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
        public string? Nombrefoto1 { get; set; }
        public string? Nombrefoto2 { get; set; }
        public string? Urlfoto1 { get; set; }
        public string? Urlfoto2 { get; set; }
        public string? Proveedor { get; set; }        
        public int? CodFinca { get; set; }
        public string? Email { get; set; }
        public int? Grupo { get; set; }
       
    }
}
