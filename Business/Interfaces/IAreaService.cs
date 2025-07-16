using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;


namespace Business.Interfaces
{
    public interface IAreaService
    {
        Task<List<TbArea>> Lista();
        Task<TbArea> Crear(TbArea entidad);
        Task<TbArea> Editar(TbArea entidad);
        Task<bool> Eliminar(int id);
    }
}
