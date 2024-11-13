using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Business.Interfaces
{
    public interface ICorreoService
    {
        Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje, byte[] archivoAdjunto, string nombreArchivo);
        Task<bool> EnviarWhatsApp(string Destino, string Mensaje, Stream archivoAdjunto = null, string nombreArchivo = "");
    }
}