using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ICheckListService
    {
        Task<List<CheckList>> Lista();
        Task<CheckList> Crear(CheckList entidad);
        Task<CheckList> Editar(CheckList entidad);
        Task<bool> Eliminar(int IdFinca);        
        Task<CheckList> ObtenerPorId(int IdRequisito);
        
    }
}
