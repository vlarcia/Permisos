using System.ComponentModel.DataAnnotations;

namespace AppRequisitos.Modelos
{

    public class Revisiones
    {
        [Key]
        public int Id_revision { get; set; }
        public int IdFinca { get; set; }
        public virtual Fincas RefFincasNombre { get; set; }
        public int IdRequisito { get; set; }
        //public virtual Checklist RefChecklist { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public string Comentarios { get; set; }        
        public bool Sincronizado {  get; set; }
        
    }
}
