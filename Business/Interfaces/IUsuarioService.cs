using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<TbUsuario>> Lista();
        Task<TbUsuario> Crear(TbUsuario entidad, Stream foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "");
        Task<TbUsuario> Editar(TbUsuario entidad, Stream foto = null, string NombreFoto = "");
        Task<bool> Eliminar(int IdUsuario);
        Task<TbUsuario> ObtenerPorCredenciales(string correo, string clave);
        Task<TbUsuario> ObtenerPorId(int IdUsuario);
        Task<bool> GuardarPerfil(TbUsuario entidad);
        Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva);
        Task<bool> RestablecerClave(string correo, string UrlPlantillaCorreo);
    }
}
