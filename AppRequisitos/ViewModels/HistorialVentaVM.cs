
using AppRequisitos.DataAccess;
using AppRequisitos.DTOs;
using AppRequisitos.Modelos;
using AppRequisitos.Services;
using AppRequisitos.Utilidades;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AppRequisitos.ViewModels
{
    public partial class HistorialVentaVM : ObservableObject
    {
       
        [ObservableProperty]
        ObservableCollection<FincasDTO> listaFinca = new ObservableCollection<FincasDTO>();

        [ObservableProperty]
        private bool loadingEsVisible = false;

        [ObservableProperty]
        private bool dataEsVisible = false;

        private readonly CumplimientoDbContext _context;
        private readonly FincaService _fincaService;
        public HistorialVentaVM(CumplimientoDbContext context, FincaService fincaservice)
        {
            _context = context;
            _fincaService = fincaservice;
            

            MainThread.BeginInvokeOnMainThread( async() =>
            {
                await ObtenerFincasSync();
            });

        }

        public async Task ObtenerFincasSync()
        {
            DataEsVisible = false;
            LoadingEsVisible = true;
            await _context.SaveChangesAsync();
            
            var lista = await _context.Fincas.OrderByDescending(v => v.IdFinca).ToListAsync();
            ListaFinca.Clear();

            if (lista.Any())
            {
                foreach (var item in lista)
                {
                    ListaFinca.Add(new FincasDTO
                    {
                        CodFinca = item.CodFinca,
                        Descripcion = item.Descripcion,
                        Proveedor = item.Proveedor,
                        Area = item.Area,
                        Encargado = item.Encargado,                        
                    });
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DataEsVisible = true;
                    LoadingEsVisible = false;
                });
            }


        }
        [RelayCommand]
        private async Task SincronizoFincas()
        {
           
            bool answer = await Shell.Current.DisplayAlert("Sincronizar", "Desea Sincronizar la tabla de Fincas?", "CONTINUAR", "CANCELAR");
            if(answer)
            {              
                await _fincaService.SincronizarFincas2();

                ListaFinca.Clear();
                await ObtenerFincasSync();
                WeakReferenceMessenger.Default.Send(new FincaMessage2());           
            }
        }
    }
    }
