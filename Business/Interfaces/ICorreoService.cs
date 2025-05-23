using Entity.DTOs;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;


namespace Business.Interfaces
{
    public interface ICorreoService
    {
        Task<bool> EnviarCorreo(string CorreoDestino, string Asunto, string Mensaje, byte[] archivoAdjunto, string nombreArchivo);
        Task<bool> EnviarWhatsApp(string Destino, string Mensaje, Stream archivoAdjunto = null, string nombreArchivo = "");
        Task<bool> MensajeWhatsApp(string Destino, string Mensaje, Stream archivoAdjunto = null, string nombreArchivo = "");
        Task<List<MensajeTwilioDTO>> ConsultarMensajesRecibidos();
        Task<bool> EnviarRespuestaWhatsApp(string destino, string textoVariable);
        
    }
}