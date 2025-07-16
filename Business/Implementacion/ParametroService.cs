using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace Business.Implementacion
{
    public class ParametroService : IParametroService
    {
        private readonly IGenericRepository<TbConfiguracion> _repositorio;
        public ParametroService(IGenericRepository<TbConfiguracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<TbConfiguracion>> Lista()
        {
            IQueryable<TbConfiguracion> query = _repositorio.Consultar();
            return await query.ToListAsync();
        }
        public async Task<TbConfiguracion> Editar(List<TbConfiguracion>  entidad)
        {
            bool respuesta = false;
            TbConfiguracion parametro_modificado = null;
            try
            {
                foreach (var parametro in entidad)
                {
                    // Busca la entidad existente en el contexto
                    parametro_modificado = await _repositorio.Obtener(p => p.IdReg == parametro.IdReg);
                    
                    if (parametro_modificado != null)   // Validar si existe
                    {
                        parametro_modificado.Valor = parametro.Valor;
                        respuesta= await _repositorio.Editar(parametro_modificado);
                        if (!respuesta)
                            throw new TaskCanceledException($"Ocurrió un error al modificar los parámetros.  Revise bien la tabla!");
                    }                    
                }
                return parametro_modificado;
            }
            catch (Exception ex)
            {                
                throw ex;
            }
        }
    }
}
