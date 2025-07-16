using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Entity.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Data.Implementacion;
//Cambios

namespace Business.Implementacion
{
    public class DashBoardService : IDashBoardService

    {
        
        private readonly IGenericRepository<TbPermiso> _permisoRepositorio;
        private readonly IGenericRepository<TbAlerta> _alertaRepositorio;
        private readonly IGenericRepository<TbDestinatario> _destinatarioRepositorio;        

        private DateTime FechaInicial=DateTime.Now;
        private DateTime FechaFinal=DateTime.Now;

        public DashBoardService(
                                IGenericRepository<TbPermiso> permisoRepositorio,
                                IGenericRepository<TbAlerta> alertaRepositorio,
                                IGenericRepository<TbDestinatario> destinatarioRepositorio)
        { 
            
            _permisoRepositorio=permisoRepositorio;
            _alertaRepositorio=alertaRepositorio;
            _destinatarioRepositorio=destinatarioRepositorio;

            FechaFinal = FechaInicial.AddDays(30);
            FechaInicial = FechaInicial.AddDays(-30);
            

        }
        public async Task<string> TotalPermisos()
        {
            try
            {
                IQueryable<TbPermiso> query = _permisoRepositorio.Consultar();
                int total = query
                    .Where(f => f.EstadoPermiso.Trim().ToUpper() != "INACTVO") // Filtro por Grupo
                    .Count(); // Cuenta solo las fincas que cumplen la condición
                return Convert.ToString(total, new CultureInfo("es-NI"));

            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalDestinatarios()
        {
            try {
                IQueryable<TbDestinatario> query = _destinatarioRepositorio.Consultar(p => p.Activo==true);
                int total = query.Count();
                
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> AlertasMes()
        {
            try
            {
                IQueryable<TbAlerta> query = _alertaRepositorio.Consultar(r => r.FechaEnvio >= FechaInicial.Date);
                int total = await query.CountAsync();
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> TotalVencimientoMes()
        {
            try
            {
                IQueryable<TbPermiso> query = _permisoRepositorio.Consultar(r => r.FechaVencimiento >= DateTime.Now && r.FechaVencimiento<=FechaFinal);
                int total = await query.CountAsync();                
                return Convert.ToString(total, new CultureInfo("es-NI"));
            }
            catch
            {
                throw;
            }
        }

        
        public async Task<Dictionary<string, int>> RenovacionesMes()
        {            
            try
            {
                IQueryable<TbPermiso> query = _permisoRepositorio
                          .Consultar(a => a.FechaCreacion >= FechaInicial.Date && a.EstadoPermiso.Trim() == "RENOVADO");
                Dictionary<string, int> resultado = await query
                    .GroupBy(a=> a.FechaCreacion).OrderByDescending(g=> g.Key)
                    .Select(da=> new {fecha=da.Key.ToString("dd/MM/yyyy"), total=da.Count()})
                    .ToDictionaryAsync(keySelector: r=> r.fecha, elementSelector: r => r.total);  
                    ;
                return resultado;            
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> VencimientosMes()
        {
            try
            {
                IQueryable<TbPermiso> query = _permisoRepositorio
                          .Consultar(r => r.FechaVencimiento >= DateTime.Now && r.FechaVencimiento <= FechaFinal);
                Dictionary<string, int> resultado = await query
                    .GroupBy(a => a.FechaVencimiento).OrderByDescending(g => g.Key)
                    .Select(da => new { fecha = da.Key.ToString("dd/MM/yyyy"), total = da.Count() })
                    .ToDictionaryAsync(keySelector: r => r.fecha, elementSelector: r => r.total);
                ;
                return resultado;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<TbPermiso>> ObtenerPermisosPorFecha(DateTime fecha)
        {
            return await _permisoRepositorio.Consultar(p => p.FechaVencimiento.Date == fecha.Date)
                .Include(p => p.IdAreaNavigation)
                .ToListAsync();
        }
        public async Task<int> TotalPermisosVencidosNoTramite()
        {
            var fechaActual = DateTime.Now;
            return await _permisoRepositorio.Consultar(p => p.FechaVencimiento < fechaActual && p.EstadoPermiso != "EN TRÁMITE").CountAsync();
        }

        public async Task<List<TbPermiso>> ObtenerPermisosVencidosSinTramite()
        {
            return await _permisoRepositorio.Consultar(p =>
                p.FechaVencimiento < DateTime.Now && p.EstadoPermiso != "EN TRÁMITE")
                .Include(p => p.IdAreaNavigation)
                .ToListAsync();
        }

    }
}
