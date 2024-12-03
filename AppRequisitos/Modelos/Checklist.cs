

using System.ComponentModel.DataAnnotations;

namespace AppRequisitos.Modelos
{
    public class Checklist
    {
        [Key]
        public int IdRequisito { get; set; }
        public string Descripcion { get; set; }
        public string Ambito { get; set; }
        public string? Norma { get; set; }
        public string? Bonsucro { get; set; }
        public string? Observaciones { get; set; }
        public virtual ICollection<Revisiones> RefRevision { get; set; } = new List<Revisiones>();
    }
}
