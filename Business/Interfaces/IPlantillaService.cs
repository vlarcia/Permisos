using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IPlantillaService
    {
        Task<string> RenderizarVistaAsync<TModel>(string nombreVista, TModel modelo);
    }
}


