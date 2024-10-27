using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IPlanesTrabajoService
    {
        //Task<List<CheckList>> ObtenerRequisito(string busqueda);
        Task<List<PlanesTrabajo>> Lista();
        Task<List<Actividad>> ListaActividad();
        Task<PlanesTrabajo> Registrar(PlanesTrabajo entidad);
        Task<List<PlanesTrabajo>> Historial(int idPlan, string fechainicio, string fechafin);
        Task<PlanesTrabajo> Detalle(int idPlan);
        Task<bool> Eliminar(int idPlan);
        Task<List<Actividad>> Reporte(string fechainicio, string fechafin);
        Task<PlanesTrabajo> Editar(PlanesTrabajo entidad);
        Task<Actividad> EditarActividad(Actividad entidad);
        Task<Actividad> RegistrarActividad(Actividad entidad);
        Task<bool> EliminarActividad(int idActividad);
       

    }
}
