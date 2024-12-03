using AppRequisitos.DataAccess;
using AppRequisitos.Pages;
using AppRequisitos.DTOs;
using AppRequisitos.Modelos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using AppRequisitos.Utilidades;


namespace AppRequisitos.ViewModels
{
    public partial class FincasVM : ObservableObject
    {
        private readonly CumplimientoDbContext _context;

        [ObservableProperty]
        private ObservableCollection<FincasDTO> listaFincas = new ObservableCollection<FincasDTO>();

        [ObservableProperty]
        private FincasDTO fincaSeleccionada;

        [ObservableProperty]
        private string buscarFinca = string.Empty;

        [ObservableProperty]
        private bool loadingEsVisible = false;

        [ObservableProperty]
        private bool dataEsVisible = false;

        [ObservableProperty]
        private bool btnLimpiarEsVisible = false;

        public FincasVM(CumplimientoDbContext context)
        {
            _context = context;
            MainThread.BeginInvokeOnMainThread(async () => await ObtenerFincas());
            PropertyChanged += FincasVM_PropertyChanged;

            WeakReferenceMessenger.Default.Register<FincaMessage2>(this, (r, m) =>
            {
                // Manejar el mensaje y actualizar la lista
                ListaFincas.Clear();
                ObtenerFincas();
            });
        }

        private void FincasVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BuscarFinca))
            {
                if (BuscarFinca != "")
                    BtnLimpiarEsVisible = true;
                else
                    BtnLimpiarEsVisible = false;
            }
        }

        public async Task ObtenerFincas()
        {
            DataEsVisible = false;
            LoadingEsVisible = true;

            await Task.Run(async () =>
            {
                await _context.SaveChangesAsync();
                
                var lstFincas = await _context.Fincas.OrderByDescending(f=> f.CodFinca).ToListAsync();
                //var lstTemp = new ObservableCollection<FincasDTO>();
                ListaFincas.Clear();

                foreach (var item in lstFincas)
                {
                    ListaFincas.Add(new FincasDTO
                    {
                        IdFinca = item.IdFinca,
                        Descripcion = item.Descripcion,
                        Proveedor=item.Proveedor,
                        Encargado = item.Encargado,
                        Area=Convert.ToDecimal(item.Area),
                        CodFinca= item.CodFinca
                    });
                }
                //ListaFincas = lstTemp;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DataEsVisible = true;
                    LoadingEsVisible = false;
                });
            });
        }

        [RelayCommand]
        private async Task Buscar()
        {
            DataEsVisible = false;
            LoadingEsVisible = true;

            await Task.Run(async () =>
            {
                ObservableCollection<FincasDTO> lista = new ObservableCollection<FincasDTO>();
                var lstFincas = await _context.Fincas.Where(c => c.Descripcion.ToLower().Contains(BuscarFinca.ToLower())).ToListAsync();

                foreach (var item in lstFincas)
                {
                    lista.Add(new FincasDTO
                    {
                        IdFinca = item.IdFinca,
                        Descripcion = item.Descripcion,
                        Proveedor=item.Proveedor,
                        Area=Convert.ToDecimal(item.Area),
                        Encargado=item.Encargado
                    });
                }
                ListaFincas = lista;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DataEsVisible = true;
                    LoadingEsVisible = false;
                });
            });
        }

        [RelayCommand]
        private async Task Limpiar()
        {
            BuscarFinca = "";
            await ObtenerFincas();
        }

        
        [RelayCommand]
        private async Task Editar(FincasDTO finca)
        {
                await Shell.Current.Navigation.PushModalAsync(new FincasEdicionPage(new FincasEdicionVM(new DataAccess.CumplimientoDbContext()), finca.IdFinca));


        }

        [RelayCommand]
        private async Task Eliminar(FincasDTO finca)
        {
            bool answer = await Shell.Current.DisplayAlert("Mensaje", "Desea eliminar la finca?", "Si, continuar", "No, volver");
            if (answer)
            {
                LoadingEsVisible = true;
                DataEsVisible = true;
                await Task.Run(async () => {
                    var encontrado = _context.Fincas.First(c => c.IdFinca == finca.IdFinca);
                    _context.Fincas.Remove(encontrado);
                    await _context.SaveChangesAsync();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaFincas.Remove(finca);
                        LoadingEsVisible = false;
                    });
                });
                
                
            }
        }
    }
}
