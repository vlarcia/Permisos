using AppRequisitos.DataAccess;
using AppRequisitos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using AppRequisitos.Services;
using AppRequisitos.DTOs;

namespace AppRequisitos.Pages
{
    public partial class SyncPage : ContentPage
    {
        private readonly CumplimientoDbContext _dbContext;
        private readonly SyncService _syncService;

        // Constructor con inyección de dependencias
        public SyncPage(CumplimientoDbContext dbContext, SyncService syncService)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _syncService = syncService;
        }

        // Sincronizar Fincas desde SQL Server
        private async void OnSyncFincasClicked(object sender, EventArgs e)
        {
            try
            {
                // Obtener las Fincas desde SQL Server
                var fincasDTO = await _syncService.GetFincasFromSqlServerAsync();

                // Limpiar la tabla de Fincas en SQLite
                _dbContext.Fincas.RemoveRange(_dbContext.Fincas);
                await _dbContext.SaveChangesAsync();

                // Convertir FincasDTO a entidades Fincas y agregar a SQLite
                var fincas = fincasDTO.Select(dto => new Fincas
                {
                    IdFinca = dto.idFinca,
                    CodFinca = dto.codFinca,
                    Descripcion = dto.descripcion,
                    Area = dto.area,
                    Encargado = dto.encargado,
                    Proveedor = dto.proveedor
                }).ToList();

                await _dbContext.Fincas.AddRangeAsync(fincas);
                await _dbContext.SaveChangesAsync();

                // Mostrar mensaje de éxito
                FincasSyncMessage.Text = "Sincronización de Fincas exitosa.";
                FincasSyncMessage.TextColor = Color.FromRgb(0, 128, 0); // Verde
            }
            catch (Exception ex)
            {
                FincasSyncMessage.Text = $"Error: {ex.Message}";
                FincasSyncMessage.TextColor = Color.FromRgb(255, 0, 0); // Rojo
            }
        }

        // Sincronizar Checklist desde SQL Server
        private async void OnSyncChecklistClicked(object sender, EventArgs e)
        {
            try
            {
                // Obtener el Checklist desde SQL Server
                var checklistDTO = await _syncService.GetChecklistFromSqlServerAsync();

                // Limpiar la tabla de Checklist en SQLite
                _dbContext.Checklist.RemoveRange(_dbContext.Checklist);
                await _dbContext.SaveChangesAsync();

                // Convertir ChecklistDTO a entidades Checklist y agregar a SQLite
                var checklist = checklistDTO.Select(dto => new Checklist
                {
                    IdRequisito = dto.idRequisito,
                    Descripcion = dto.descripcion,
                    Ambito = dto.ambito,
                    Norma = dto.norma,
                    Bonsucro = dto.bonsucro,
                    Observaciones = dto.observaciones
                }).ToList();

                await _dbContext.Checklist.AddRangeAsync(checklist);
                await _dbContext.SaveChangesAsync();

                // Mostrar mensaje de éxito
                ChecklistSyncMessage.Text = "Sincronización de Checklist exitosa.";
                ChecklistSyncMessage.TextColor = Color.FromRgb(0, 128, 0); // Verde
            }
            catch (Exception ex)
            {
                ChecklistSyncMessage.Text = $"Error: {ex.Message}";
                ChecklistSyncMessage.TextColor = Color.FromRgb(255, 0, 0); // Rojo
            }
        }
    }
}
