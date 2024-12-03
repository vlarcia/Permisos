using AppRequisitos.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.Utilidades
{
    public class FincaResult
    {
        public bool esCrear { get; set; }
        public FincasDTO finca { get; set; }
    }
}
