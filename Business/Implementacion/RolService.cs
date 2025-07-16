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
    public class RolService : IRolService
    {
        private readonly IGenericRepository<TbRol> _repositorio;
        public RolService(IGenericRepository<TbRol> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<TbRol>> Lista()
        {
           IQueryable<TbRol> query = _repositorio.Consultar();
            return await query.ToListAsync();  
        }
    }
}
