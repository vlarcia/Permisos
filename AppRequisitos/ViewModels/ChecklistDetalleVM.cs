using AppRequisitos.DataAccess;
using AppRequisitos.DTOs;
using AppRequisitos.Modelos;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AppRequisitos.ViewModels
{
    public partial class ChecklistDetalleVM : ObservableObject
    {
        private readonly CumplimientoDbContext _context;
        private readonly int _idRequisito;

        // Propiedades vinculadas a la vista
        [ObservableProperty]
        private bool loadingEsVisible = true;

        [ObservableProperty]
        private int idRequisito;

        [ObservableProperty]
        private string descripcion;

        [ObservableProperty]
        private string ambito;

        [ObservableProperty]
        private string norma;

        [ObservableProperty]
        private string bonsucro;

        [ObservableProperty]
        private string observaciones;

        [ObservableProperty]
        private string tituloPagina;
        private CumplimientoDbContext cumplimientoDbContext;

        public ChecklistDetalleVM(CumplimientoDbContext cumplimientoDbContext)
        {
            this.cumplimientoDbContext = cumplimientoDbContext;
        }

        /// <summary>
        /// Constructor que recibe el id del requisito
        /// </summary>
        public ChecklistDetalleVM(CumplimientoDbContext context, int idRequisito)
        {
            _context = context;
            _idRequisito = idRequisito;

            // Llamamos inmediatamente a CargarDetalles
            CargarDetalles();
        }

        /// <summary>
        /// Carga los detalles del requisito seleccionado
        /// </summary>
        public async void CargarDetalles()
        {
            TituloPagina = "Detalles del Requisito";
            LoadingEsVisible = true;
            LoadingEsVisible = true;

            var encontrado = await _context.Checklist.FirstOrDefaultAsync(p => p.IdRequisito == _idRequisito);

            if (encontrado != null)
            {
                Descripcion = encontrado.Descripcion;
                Ambito = encontrado.Ambito;
                Norma = encontrado.Norma;
                Bonsucro = encontrado.Bonsucro;
                Observaciones = encontrado.Observaciones;
            }

            LoadingEsVisible = false;

        }
    }
}
