using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IVisitaService
    {
        
        Task<List<Visita>> Lista(int envio=0);        
        Task<Visita> Registrar(Visita entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "", Stream foto3 = null, string NombreFoto3 = "", Stream foto4 = null, string NombreFoto4 = "");
        Task<Visita> Editar(Visita entidad, Stream foto1 = null, string NombreFoto1 = "", Stream foto2 = null, string NombreFoto2 = "", Stream foto3 = null, string NombreFoto3 = "", Stream foto4 = null, string NombreFoto4 = "");        
        Task<bool> Eliminar(int idVisita);
        Task<Visita> Detalle(int idVisita);
        Task<List<DetalleVisita>> ListaDetalle();
    }
}
