using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IDestinatarioService
    {
        Task<List<TbDestinatario>> Lista();
        Task<List<TbDestinatario>> ListaPorArea(int idArea);
        Task<TbDestinatario> Crear(TbDestinatario entidad);
        Task<TbDestinatario> Editar(TbDestinatario entidad);
        Task<bool> Eliminar(int idDestinatario);
        Task<TbDestinatario> ObtenerPorId(int IdDestinatario);
        Task<List<TbPermisoDestinatario>> ListaPermisoDestinatario();
    }
}
