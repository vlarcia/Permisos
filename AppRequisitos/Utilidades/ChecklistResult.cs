using AppRequisitos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.Utilidades
{
    public class ChecklistResult
    {
        public bool esCrear { get; set; }
        public ChecklistDTO requisito { get; set; }
    }
}
