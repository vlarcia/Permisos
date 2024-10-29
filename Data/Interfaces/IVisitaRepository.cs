using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IVisitaRepository: IGenericRepository<Visita>
    {
        Task<Visita> Registrar(Visita entidad);
        Task<List<DetalleVisita>> Reporte(DateTime Fechainicio, DateTime Fechafin);
    }
}
