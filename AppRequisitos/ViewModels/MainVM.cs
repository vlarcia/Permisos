using AppRequisitos.DataAccess;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.ViewModels
{
    public partial class MainVM :ObservableObject
    {
        private readonly CumplimientoDbContext _context;
        public MainVM(CumplimientoDbContext context)
        {
            _context = context;
            MainThread.BeginInvokeOnMainThread(async() =>
            {
                await Task.Run(async () => await ObtenerResumen());
            });
        }

        [ObservableProperty]
        private decimal totalIngresos;

        [ObservableProperty]
        private int totalVentas;

        [ObservableProperty]
        private int totalProductos;

        [ObservableProperty]
        private int totalCategorias;

        private async Task ObtenerResumen()
        {
            decimal totalingresos = 0;
            var lstVentas = await _context.Ventas.ToListAsync();
            foreach (var item in lstVentas)
            {
                totalingresos += item.Total;
            }

            TotalIngresos = _context.Fincas.Count();
                
            TotalVentas =  _context.Actividades.Count();
            TotalProductos =  _context.Fincas.Count();
            TotalCategorias =  _context.Checklist.Count();
        }

    }
}
