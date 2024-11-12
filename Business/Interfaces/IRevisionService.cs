using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRevisionService
    {
        Task<List<CheckList>> ListaRequisitos();
        Task<List<Revisione>> Lista();        
        Task<Revisione> Crear(List<Revisione> entidad);
        Task<Revisione> Editar(List<Revisione> entidad);        
        Task<bool> Eliminar(List<int> entidad, int entidad2=0);  
        Task<List<Revisione>> ObtenerRevision(int idfinca=0, string fecha="", int grupo=0);
        Task<List<Revision>> ListaRevisions();
        Task<Revision> CrearRevisions(Revision entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "");
        Task<Revision> EditarRevisions(Revision entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "");
        Task<Revision> ObtenerRevisionGeneral(int idfinca = 0, string fecha = "");
    }
}
