using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
   public class MensajeTwilioDTO
    {
        public string? Sid { get; set; }
        public string? Fecha { get; set; }
        public string? Numero { get; set; }
        public string? Mensaje { get; set; }
        public string? Estado { get; set; }
    }
}
