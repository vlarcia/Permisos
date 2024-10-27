using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace Business.Implementacion
{
    public class MaestroFincaService : IMaestroFincaService
    {
        private readonly IGenericRepository<MaestroFinca> _repositorio;        
        
    
        public MaestroFincaService(
            IGenericRepository<MaestroFinca> repositorio)
        {
            _repositorio = repositorio;                        

        }
        public async Task<List<MaestroFinca>> Lista()
        {
            IQueryable<MaestroFinca> query = await _repositorio.Consultar();
            return query.ToList();    
        }
        public async Task<MaestroFinca> Crear(MaestroFinca entidad)
        {
            MaestroFinca finca_existe = await _repositorio.Obtener(u => u.CodFinca == entidad.CodFinca);
            if (finca_existe != null)
                throw new TaskCanceledException("Esa Finca ya existe");
            try
            {                
                MaestroFinca finca_creada = await _repositorio.Crear(entidad);
                if (finca_creada.IdFinca == 0)
                    throw new TaskCanceledException("No se pudo crear la Finca!");
                
                IQueryable<MaestroFinca> query=await _repositorio.Consultar(u=> u.IdFinca == finca_creada.IdFinca);
                finca_creada=query.First();

                return finca_creada;

            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public async Task<MaestroFinca> Editar(MaestroFinca entidad)
        {
            MaestroFinca finca_existe = await _repositorio.Obtener(u => u.CodFinca == entidad.CodFinca && u.IdFinca != entidad.IdFinca);
            if (finca_existe != null)
                throw new TaskCanceledException("El codigo de finca ya existe en otro registro");
            try
            {
                IQueryable<MaestroFinca> queryFinca = await _repositorio.Consultar(u => u.IdFinca == entidad.IdFinca);
                MaestroFinca finca_editar = queryFinca.First();
                finca_editar.CodFinca = entidad.CodFinca;
                finca_editar.Descripcion = entidad.Descripcion;                
                finca_editar.Area = entidad.Area;                
                finca_editar.Proveedor = entidad.Proveedor;
                finca_editar.Email = entidad.Email;
                finca_editar.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(finca_editar);
                if(!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar la finca.  Revise !");
                MaestroFinca finca_editada=queryFinca.First();
                return finca_editada;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdFinca)
        {
            try
            {
                MaestroFinca finca_encontrada = await _repositorio.Obtener(u => u.IdFinca == IdFinca);
                if (finca_encontrada == null)
                    throw new TaskCanceledException("La Finca no existe!");
                
                bool respuesta = await _repositorio.Eliminar(finca_encontrada);                    
                return true;
            }
            catch
            {
                throw;
            }
        }       

        public async Task<MaestroFinca> ObtenerPorCodigo(int codfinca)
        {
            
            MaestroFinca finca_encontarda = await _repositorio.Obtener(u=>u.CodFinca == codfinca);
            return finca_encontarda;  
        }

        public async Task<MaestroFinca> ObtenerPorId(int IdFinca)
        {
            IQueryable<MaestroFinca> query = await _repositorio.Consultar(u => u.IdFinca == IdFinca);
                MaestroFinca resultado=query.First();
            return resultado;

        }
        
    }
}
