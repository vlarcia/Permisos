using AppRequisitos.DataAccess;
using AppRequisitos.DTOs;
using AppRequisitos.Modelos;
using AppRequisitos.Pages;
using AppRequisitos.Utilidades;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Formats.Tar;
using System.IO;
using System.Reflection;

namespace AppRequisitos.ViewModels
{

    public partial class FincasEdicionVM : ObservableObject
    {
        private readonly CumplimientoDbContext _context;
        public FincasEdicionVM(CumplimientoDbContext context)
        {
            WeakReferenceMessenger.Default.Register<BarcodeScannedMessage>(this, (r, m) =>
            {
                BarcodeMensajeRecibido(m.Value);
            });
            _context = context;
        }

        private int IdFinca;
        [ObservableProperty]
        private bool loadingEsVisible = false;

        [ObservableProperty]
        private string codigoBarras = string.Empty;

        [ObservableProperty]
        private string descripcion = string.Empty;

        [ObservableProperty]
        private CategoriaDTO categoriaSeleccionada;

        [ObservableProperty]
        private decimal area;

        [ObservableProperty]
        private string proveedor;
        [ObservableProperty]
        private string encargado;

        [ObservableProperty]
        private int codFinca;

        [ObservableProperty]
        private string tituloPagina;

        public async void Inicio(int idFinca)
        {
            IdFinca = idFinca;



            if (IdFinca == 0)
            {
                TituloPagina = "Agregar Finca";
                
            }
            else
            {
                TituloPagina = "Editar Finca";
                LoadingEsVisible = false;

                await Task.Run(async () =>
                {
                    var encontrado = await _context.Fincas.FirstAsync(p => p.IdFinca == IdFinca);
                    
                    Descripcion = encontrado.Descripcion;
                    Proveedor=encontrado.Proveedor;
                    Area = Convert.ToDecimal(encontrado.Area);
                    Encargado = encontrado.Encargado;
                    CodFinca=encontrado.CodFinca;      
                });

            }
        }
        private void BarcodeMensajeRecibido(BarcodeResult result)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CodigoBarras = result.BarcodeValue;
            });
        }

        
        [RelayCommand]
        private async Task MostrarScanner()
        {
            await Shell.Current.Navigation.PushModalAsync(new BarcodePage());
        }

        [RelayCommand]
        private async Task VolverInventario()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        [RelayCommand]
        private async Task Guardar()
        {
            LoadingEsVisible = true;
            FincaResult fincaResult = new FincaResult();

            var enviarFinca = new FincasDTO
            {
                IdFinca = IdFinca,
                Descripcion=Descripcion,
                Proveedor = Proveedor,
                Area=Area,
                Encargado=Encargado
            };

            await Task.Run(async () =>
            {

                if (IdFinca == 0)
                {
                    var dbFinca = new Fincas
                    {                        
                        IdFinca = IdFinca, 
                        Descripcion = CodigoBarras,
                        Proveedor = Proveedor,
                        Encargado = Encargado,
                        Area = Area,
                        CodFinca=CodFinca,
                        
                    };
                    _context.Fincas.Add(dbFinca);

                    await _context.SaveChangesAsync();

                    enviarFinca.IdFinca = dbFinca.IdFinca;

                    fincaResult = new FincaResult()
                    {
                        esCrear = true,
                        finca = enviarFinca
                    };

                }
                else
                {
                    var encontrado = await _context.Fincas.FirstAsync(p => p.IdFinca == IdFinca);
                    encontrado.IdFinca= IdFinca;                    
                    encontrado.Descripcion = Descripcion;
                    encontrado.CodFinca = CodFinca;
                    encontrado.Area = Area;
                    encontrado.Encargado = Encargado;
                    encontrado.Proveedor = Proveedor;
                    _context.Fincas.Update(encontrado);
                    await _context.SaveChangesAsync();

                    fincaResult = new FincaResult()
                    {
                        esCrear = false,
                        finca = enviarFinca
                    };
                }
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    LoadingEsVisible = false;
                    WeakReferenceMessenger.Default.Send(new FincaMessage(fincaResult));
                    await Shell.Current.Navigation.PopModalAsync();
                });
            });

        }

    }
}
