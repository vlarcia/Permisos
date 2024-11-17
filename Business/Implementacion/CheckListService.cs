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
    public class CheckListService : ICheckListService
    {
        private readonly IGenericRepository<CheckList> _repositorio;        
        
    
        public CheckListService(
            IGenericRepository<CheckList> repositorio)
        {
            _repositorio = repositorio;                        

        }
        public async Task<List<CheckList>> Lista()
        {
            IQueryable<CheckList> query = await _repositorio.Consultar();
            return query.ToList();    
        }
        public async Task<CheckList> Crear(CheckList entidad)
        {
            CheckList req_existe = await _repositorio.Obtener(u => u.IdRequisito == entidad.IdRequisito );
            if (req_existe != null)
                throw new TaskCanceledException("Ese Requerimiento ya existe");
            try
            {                
                CheckList req_creada = await _repositorio.Crear(entidad);
                if (req_creada.IdRequisito == 0)
                    throw new TaskCanceledException("No se pudo crear el nuevo requerimiento!");
                
                IQueryable<CheckList> query=await _repositorio.Consultar(u=> u.IdRequisito == req_creada.IdRequisito);
                req_creada=query.First();

                return req_creada;
            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public async Task<CheckList> Editar(CheckList entidad)
        {
            CheckList req_existe = await _repositorio.Obtener(u => u.IdRequisito == entidad.IdRequisito);
            if (req_existe == null)
                throw new TaskCanceledException("El codigo de Requerimiento no existe!");
            try
            {
                IQueryable<CheckList> queryReq = await _repositorio.Consultar(u => u.IdRequisito == entidad.IdRequisito);
                CheckList req_editar = queryReq.First();                
                req_editar.Descripcion = entidad.Descripcion;                
                req_editar.Ambito = entidad.Ambito;                
                req_editar.Norma = entidad.Norma;
                req_editar.Bonsucro = entidad.Bonsucro;
                req_editar.Observaciones = entidad.Observaciones;

                bool respuesta = await _repositorio.Editar(req_editar);
                if(!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar el requisito.  Revise !");
                CheckList req_editada=queryReq.First();
                return req_editada;
            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdRequisito)
        {
            try
            {
                CheckList req_encontrada = await _repositorio.Obtener(u => u.IdRequisito == IdRequisito);
                if (req_encontrada == null)
                    throw new TaskCanceledException("El requerimiento no existe!");
                
                bool respuesta = await _repositorio.Eliminar(req_encontrada);                    
                return true;
            }
            catch
            {
                throw;
            }
        }       

    
        public async Task<CheckList> ObtenerPorId(int IdRequisito)
        {
            IQueryable<CheckList> query = await _repositorio.Consultar(u => u.IdRequisito == IdRequisito);
                CheckList resultado=query.First();
            return resultado;

        }
        
    }
}
