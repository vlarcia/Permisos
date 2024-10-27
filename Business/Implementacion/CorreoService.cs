using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Business.Interfaces;
using Data.Interfaces;
using Entity;

namespace Business.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        public CorreoService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<bool> EnviarCorreo(string CorreosDestino, string Asunto, string Mensaje, byte[] archivoAdjunto = null, string nombreArchivo = null)
        {
            try
            {
                // Obtener la configuración desde la base de datos
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Servicio_Correo"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var credenciales = new NetworkCredential(Config["correo"], Config["clave"]);
                var correo = new MailMessage()
                {
                    From = new MailAddress(Config["correo"], Config["alias"]),
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                // Separar los correos destino usando el delimitador ";" 
                string[] destinatarios = CorreosDestino.Split(';');

                foreach (var destinatario in destinatarios)
                {
                    // Agregar cada destinatario solo si no está vacío o en blanco
                    if (!string.IsNullOrWhiteSpace(destinatario))
                    {
                        correo.To.Add(new MailAddress(destinatario.Trim()));
                    }
                }

                // Si se proporciona un archivo adjunto
                if (archivoAdjunto != null && !string.IsNullOrEmpty(nombreArchivo))
                {
                    var ms = new MemoryStream(archivoAdjunto);
                    // Agregar el archivo adjunto al correo sin cerrar el MemoryStream
                    correo.Attachments.Add(new Attachment(ms, nombreArchivo, "application/pdf"));
                }

                // Configurar el servidor SMTP
                var clienteServidor = new SmtpClient()
                {
                    Host = Config["host"],
                    Port = int.Parse(Config["puerto"]),
                    Credentials = credenciales,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true
                };

                clienteServidor.Send(correo);  // Enviar el correo

                // Cerrar el MemoryStream solo después de enviar el correo
                if (archivoAdjunto != null)
                {
                    correo.Attachments.Dispose();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Opcionalmente puedes registrar el error
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
