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
        Task<string> TotalFincas();
        Task<string> TotalPlanesActivos();
        Task<string> TotalActividadesActivas();
        Task<string> RevisionesUltimoMes();
        Task<string> VisitasUltimoMes();
        Task<Dictionary<string, int>> ActividadesCompletadasUltimoMes();
        Task<Dictionary<string,int>> FincasVisitadasUltimoMes();
        Task<List<CumplimientoDTO>> ObtenerFincasConCumplimiento();

    }
}
