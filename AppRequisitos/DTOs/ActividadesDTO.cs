using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.DTOs
{
    public partial class ActividadesDTO: ObservableObject
    {
        [ObservableProperty]
        public int id_actividad;
        [ObservableProperty]
        public int id_plan;
        [ObservableProperty]
        public int idFinca;
        [ObservableProperty]        
        public string descripciion;
        [ObservableProperty]
        public string tipo;
        [ObservableProperty]
        public DateTime fechaini;
        public DateTime fechafin;
        [ObservableProperty]
        public decimal avances;
        [ObservableProperty]
        public decimal avanceanterior;
        [ObservableProperty]
        public string estado;
        [ObservableProperty]
        public string responsable;
        [ObservableProperty]
        public string observaciones;
        [ObservableProperty]
        public bool sincronizado;
    }
}
