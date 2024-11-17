using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface ISincronizaService
    {
        Task<List<AndroidId>> ListaAndroid();
        Task<List<Visita>> ListaVisita();
        Task<AndroidId> Crear(AndroidId entidad);
        Task<AndroidId> Editar(AndroidId entidad);
        Task<bool> Eliminar(int idAndroidId);
        Task<Visita> AplicaVisita(Visita entidad);
    }
}
