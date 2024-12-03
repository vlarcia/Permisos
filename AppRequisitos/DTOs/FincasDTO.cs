using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.DTOs
{
    public partial class FincasDTO: ObservableObject
    {
        [ObservableProperty]
        public int idFinca;
        [ObservableProperty]
        public int codFinca;
        [ObservableProperty]
        public string descripcion;
        [ObservableProperty]
        public decimal? area;
        [ObservableProperty]
        public string? encargado;
        [ObservableProperty]
        public string proveedor;
    }
}
