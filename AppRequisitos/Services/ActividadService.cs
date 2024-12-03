using AppRequisitos.DataAccess;
using AppRequisitos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace AppRequisitos.Services
{
    // ActividadService.cs
    public class ActividadService
    {
        private readonly CumplimientoDbContext _dbContext;
        private readonly HttpClient _httpClient;

        public ActividadService(CumplimientoDbContext dbContext)
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient();
        }

        public async Task SincronizarActividades()
        {
            var response = await _httpClient.GetAsync("https://localhost:5249/api/actividades");
            if (response.IsSuccessStatusCode)
            {
                var actividades = await response.Content.ReadFromJsonAsync<List<Actividades>>();
                if (actividades != null)

                {
                    await ClearActividades();

                    foreach (var actividad in actividades)
                    {
                        _dbContext.Actividades.Add(actividad);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        public async Task ClearActividades()
        {
            var allActividades = await _dbContext.Actividades.ToListAsync();
            _dbContext.Actividades.RemoveRange(allActividades);
            await _dbContext.SaveChangesAsync();
        }

    }

}


