using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.DTOs
{
    public partial class RevisionesDTO: ObservableObject
    {
        [ObservableProperty]
        public int id_revision;
        [ObservableProperty]
        public int idFinca;
        [ObservableProperty]
        public int id_requisito;
        [ObservableProperty]
        public string descripcion;
        [ObservableProperty]
        public DateTime fecha;
        [ObservableProperty]
        public string estado;        
        [ObservableProperty]
        public string comentarios;
        [ObservableProperty]
        public bool sincronizado;
    }
}
