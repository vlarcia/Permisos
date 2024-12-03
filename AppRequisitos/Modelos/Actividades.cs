using System.ComponentModel.DataAnnotations;

namespace AppRequisitos.Modelos
{
    public class Actividades
    {
        [Key]
        public int Id_actividad { get; set; }
        public int Id_plan { get; set; }
        public int IdFinca { get; set; }
        public virtual Fincas RefFincasNombre { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public DateTime Fechaini { get; set; }
        public DateTime Fechafin { get; set; }
        public decimal Avances { get; set; }
        public decimal Avanceanterior { get; set; }
        public string Estado { get; set; }
        public string Responsable { get; set; }
        public string Observaciones { get; set; }
        public bool Sincronizado { get; set; }
    }
}
