using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IPlanesRepository: IGenericRepository<PlanesTrabajo>
    {
        Task<PlanesTrabajo> Registrar(PlanesTrabajo entidad);
        Task<List<Actividad>> Reporte(DateTime Fechainicio, DateTime Fechafin);
    }
}
