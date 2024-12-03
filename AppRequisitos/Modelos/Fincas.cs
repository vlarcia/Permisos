using System.ComponentModel.DataAnnotations;

namespace AppRequisitos.Modelos
{

    public class Fincas
    {
        [Key]
        public int IdFinca { get; set; }
        public int CodFinca { get; set; }
        public string Descripcion { get; set; }
        public decimal? Area { get; set; }
        public string? Encargado { get; set; }
        public string Proveedor { get; set; }
        public virtual ICollection<Revisiones> RefFincas { get; set; } = new List<Revisiones>();
        public virtual ICollection<Actividades> RefFincas2 { get; set; } = new List<Actividades>();
    }


}
