using Entity;
using Entity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IDashBoardService
    {
        Task<string> TotalPermisos();
        Task<string> TotalDestinatarios();        
        Task<string> AlertasMes();
        Task<string> TotalVencimientoMes();
        Task<Dictionary<string, int>> RenovacionesMes();
        Task<Dictionary<string,int>> VencimientosMes();
        Task<List<TbPermiso>> ObtenerPermisosPorFecha(DateTime fecha);
        Task<int> TotalPermisosVencidosNoTramite();
        Task<List<TbPermiso>> ObtenerPermisosVencidosSinTramite();




    }
}
