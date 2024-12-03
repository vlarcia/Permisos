using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.DTOs
{
    public partial class ChecklistDTO: ObservableObject
    {
        [ObservableProperty]
        public int idRequisito;
        [ObservableProperty]
        public string descripcion;
        [ObservableProperty]
        public string ambito;
        [ObservableProperty]
        public string? norma;
        [ObservableProperty]
        public string? bonsucro;
        [ObservableProperty]
        public string? observaciones;
    }
}
