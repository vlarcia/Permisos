using System.ComponentModel.DataAnnotations;

namespace AppRequisitos.Models
{
    public class Usuarios
    {
        [Key]
        public int Iduser { get; set; }
        public string Nombre { get; set; }    
        public string Login { get; set; }
        public string Passwd { get; set; }
        public string AndroidId { get; set; }
        public DateTime Fechaing { get; set; }
    
    }
}
