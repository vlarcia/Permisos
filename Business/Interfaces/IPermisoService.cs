using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IPermisoService
    {
        Task<List<TbPermiso>> Lista();
        Task<List<TbPermiso>> ListaPorArea(int idArea);
        Task<TbPermiso> Crear(TbPermiso entidad, Stream archivoEvidencia = null, string nombreEvidencia = "",
                                                 Stream archivoEvidencia2 = null, string nombreEvidencia2 = "",
                                                 Stream archivoEvidencia3 = null, string nombreEvidencia3 = "");
        Task<TbPermiso> Editar(TbPermiso entidad, Stream archivoEvidencia = null, string nombreEvidencia = "",
                                                  Stream archivoEvidencia2 = null, string nombreEvidencia2 = "",
                                                  Stream archivoEvidencia3 = null, string nombreEvidencia3 = "",
                                                  bool eliminar1 = false,
                                                  bool eliminar2 = false,
                                                  bool eliminar3 = false);
        Task<bool> Eliminar(int IdPermiso);        
        Task<TbPermiso> ObtenerPorId(int IdPermiso);

    }
}
