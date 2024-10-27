using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IMaestroFincaService
    {
        Task<List<MaestroFinca>> Lista();
        Task<MaestroFinca> Crear(MaestroFinca entidad);
        Task<MaestroFinca> Editar(MaestroFinca entidad);
        Task<bool> Eliminar(int IdFinca);
        Task<MaestroFinca> ObtenerPorCodigo(int CodFinca);
        Task<MaestroFinca> ObtenerPorId(int IdFinca);
        
    }
}
