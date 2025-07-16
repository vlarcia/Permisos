using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IAlertaService
    {
        Task<List<TbAlerta>> Lista();
        Task<TbAlerta> Crear(TbAlerta entidad, bool permitirDuplicado = false);
        Task<TbAlerta> Editar(TbAlerta entidad);
        Task<bool> Eliminar(int IdAlerta);
        Task<TbAlerta> ObtenerPorId(int IdAlerta);
        Task<bool> ExisteAlertaRecienteAsync(int idPermiso, int idDestinatario);

    }
}
