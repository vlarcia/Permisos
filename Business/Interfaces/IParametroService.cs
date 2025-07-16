using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IParametroService
    {
        Task<List<TbConfiguracion>> Lista();
        Task<TbConfiguracion> Editar(List<TbConfiguracion> entidad);
    }    
}
