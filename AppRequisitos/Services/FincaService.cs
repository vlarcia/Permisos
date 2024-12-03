using AppRequisitos.DataAccess;
using AppRequisitos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.PlatformConfiguration.TizenSpecific;

namespace AppRequisitos.Services
{
    // FincaService.cs
    public class FincaService
    {
        private readonly SQLServerDbContext _dbContextSQL;
        private readonly CumplimientoDbContext _dbContextLite;
        private readonly HttpClient _httpClient;
        //private ProgressBar _progressBar;
        //private double _progress;
        public ObservableCollection<Fincas> Fincas { get; private set; }
        public event Action FincasChanged;
        public FincaService(SQLServerDbContext dbContextSQL, CumplimientoDbContext dbContextLite, HttpClient httpClient)
        {
            _dbContextSQL = dbContextSQL;
            _dbContextLite = dbContextLite;
            _httpClient = httpClient;
            Fincas = new ObservableCollection<Fincas>(_dbContextLite.Fincas.ToList());
        }

        public async Task SincronizarFincas()
        {
              
            var response = await _httpClient.GetAsync("http://10.0.2.2:5249/api/Fincas");
            if (response.IsSuccessStatusCode)
            {
                var fincas = await response.Content.ReadFromJsonAsync<List<Fincas>>();
                if (fincas != null)

                {
                    await ClearFincas();
                    
                    foreach (var finca in fincas)
                    {
                        _dbContextLite.Fincas.Add(finca);
                    }
                    Fincas.Clear();
                    foreach (var finca in fincas)
                    {
                        Fincas.Add(finca);
                    }

                    await _dbContextLite.SaveChangesAsync();
                    _dbContextLite.ChangeTracker.Clear();

                    await Shell.Current.DisplayAlert("Mensaje", "Tabla de Fincas Sincronizada con éxito!", "Ok, continuar");
                }
            }
        }
        public async Task SincronizarFincas2()
        {
            try
            {
                //_progressBar.IsVisible=true; // Show progress bar before operation

                // Consulta directa a la base de datos SQL Server usando Entity Framework Core
                var fincasFromSQL = await _dbContextSQL.Fincas.AsNoTracking().ToListAsync();

                if (fincasFromSQL != null && fincasFromSQL.Count > 0)
                {
                    await ClearFincas();

                    foreach (var finca in fincasFromSQL)
                    {
                        _dbContextLite.Fincas.Add(finca);
                    }

                    Fincas.Clear();
                    foreach (var finca in fincasFromSQL)
                    {
                        Fincas.Add(finca);
                    }

                    await _dbContextLite.SaveChangesAsync();
                    _dbContextLite.ChangeTracker.Clear();

                    await Shell.Current.DisplayAlert("Mensaje", "Tabla de Fincas sincronizada con éxito desde SQL Server!", "Ok, continuar");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Advertencia", "No se encontraron registros en la base de datos.", "Ok");
                }
                //_progressBar.IsVisible = false; // Hide progress bar after completion
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error durante la sincronización: {ex.Message}", "Ok");
                //_progressBar.IsVisible = false;
            }
        }

        public async Task ClearFincas()
        {
            var allFincas = await _dbContextLite.Fincas.ToListAsync();
            _dbContextLite.Fincas.RemoveRange(allFincas);
            await _dbContextLite.SaveChangesAsync();
        }
        // Update progress within the loop if applicable
        private void UpdateProgress(int downloadedCount, int totalCount)
        {
            //_progress = (double)downloadedCount / totalCount;
            //_progressBar.Progress = _progress;
        }
    }

}
