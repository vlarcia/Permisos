using AppRequisitos.DataAccess;
using AppRequisitos.Pages;
using AppRequisitos.DTOs;
using AppRequisitos.Modelos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;


namespace AppRequisitos.ViewModels
{
    public partial class ChecklistVM : ObservableObject
    {
        private readonly CumplimientoDbContext _context;

        [ObservableProperty]
        private ObservableCollection<ChecklistDTO> listaRequisitos = new ObservableCollection<ChecklistDTO>();

        [ObservableProperty]
        private ChecklistDTO requisitoSeleccionado;

        [ObservableProperty]
        private string buscarRequisito = string.Empty;

        [ObservableProperty]
        private bool loadingEsVisible = false;

        [ObservableProperty]
        private bool dataEsVisible = false;

        [ObservableProperty]
        private bool btnLimpiarEsVisible = false;

        public ChecklistVM(CumplimientoDbContext context)
        {
            _context = context;
            MainThread.BeginInvokeOnMainThread(async () => await ObtenerRequisitos());
            PropertyChanged += ChecklistVM_PropertyChanged;
        }

        private void ChecklistVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BuscarRequisito))
            {
                if (BuscarRequisito != "")
                    BtnLimpiarEsVisible = true;
                else
                    BtnLimpiarEsVisible = false;
            }
        }

        public async Task ObtenerRequisitos()
        {
            DataEsVisible = false;
            LoadingEsVisible = true;

            await Task.Run(async () =>
            {

                var lstRequisitos = await _context.Checklist.ToListAsync();
                var lstTemp = new ObservableCollection<ChecklistDTO>();

                foreach (var item in lstRequisitos)
                {
                    lstTemp.Add(new ChecklistDTO
                    {
                        IdRequisito = item.IdRequisito,
                        Descripcion = item.Descripcion,
                        Ambito = item.Ambito,
                        Norma=item.Norma,
                        Bonsucro = item.Bonsucro,
                        Observaciones = item.Observaciones                        
                    });
                }
                ListaRequisitos = lstTemp;

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
                ObservableCollection<ChecklistDTO> lista = new ObservableCollection<ChecklistDTO>();
                var lstRequisitos = await _context.Checklist.Where(c => c.Descripcion.ToLower().Contains(BuscarRequisito.ToLower())).ToListAsync();

                foreach (var item in lstRequisitos)
                {
                    lista.Add(new ChecklistDTO
                    {
                        IdRequisito = item.IdRequisito,
                        Descripcion = item.Descripcion,
                        Ambito=item.Ambito
                    });
                }
                ListaRequisitos = lista;
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
            BuscarRequisito = "";
            await ObtenerRequisitos();
        }

        [RelayCommand]
        private async Task Agregar()
        {

            string resultado1 = await Shell.Current.DisplayPromptAsync("Nuevo Requsisto", "Ingrese el nombre", accept: "Guardar", cancel: "Volver");
            string resultado2 = await Shell.Current.DisplayPromptAsync("Nuevo Requisito", "Ingrese el Ambito", accept: "Guardar", cancel: "Volver");
            if (!string.IsNullOrEmpty(resultado1))
            {
                LoadingEsVisible = true;
                DataEsVisible = true;
                await Task.Run(async () =>
                {
                    Checklist modelo = new Checklist
                    {
                        Descripcion = resultado1,
                        Ambito = resultado2  
                    };
                    _context.Checklist.Add(modelo);
                    await _context.SaveChangesAsync();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaRequisitos.Add(new ChecklistDTO { IdRequisito = modelo.IdRequisito, Descripcion = modelo.Descripcion });
                        LoadingEsVisible = false;
                    });
                });
            }

        }

        [RelayCommand]
        private async Task Mostrar(ChecklistDTO requisito)
        {
            await Shell.Current.Navigation.PushModalAsync(new ChecklistDetallePage(new ChecklistDetalleVM(new DataAccess.CumplimientoDbContext()), requisito.IdRequisito));       
        }

        [RelayCommand]
        private async Task Eliminar(ChecklistDTO requisito)
        {
            bool answer = await Shell.Current.DisplayAlert("Mensaje", "Desea eliminar el requisito?", "Si, continuar", "No, volver");
            if (answer)
            {
                LoadingEsVisible = true;
                DataEsVisible = true;
                await Task.Run(async () => {
                    var encontrado = _context.Checklist.First(c => c.IdRequisito == requisito.IdRequisito);
                    _context.Checklist.Remove(encontrado);
                    await _context.SaveChangesAsync();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaRequisitos.Remove(requisito);
                        LoadingEsVisible = false;
                    });
                });
                
                
            }
        }
    }
}
