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
    public class TipoInformeService : ITipoInformeService
    {
        private readonly IGenericRepository<TipoInforme> _repositorio;
        
        public TipoInformeService(
            IGenericRepository<TipoInforme> repositorio
            )
        {
            _repositorio = repositorio;            

        }
        public async Task<List<TipoInforme>> Lista()
        {
            IQueryable<TipoInforme> query = await _repositorio.Consultar();
            return query.ToList();    
        }
      
    }
}
