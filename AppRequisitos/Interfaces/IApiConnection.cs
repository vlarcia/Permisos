using AppRequisitos.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRequisitos.Interfaces
{
    public interface IApiConnection
    {
        [Post("/api/Usuarios/GetUsuarios/")]
        Task<Usuarios> GetUsuarios();
    }
}
