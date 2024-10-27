using Entity;

namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMMaestroFinca
    {
        public int IdFinca { get; set; }

        public int? CodFinca { get; set; }

        public string? Descripcion { get; set; }

        public string? Proveedor { get; set; }

        public string? Email { get; set; }
        public decimal? Area { get; set; }

        public string? Telefono { get; set; }        

    }
}
