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
using Microsoft.Identity.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Business.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        private readonly IFireBaseService _firebaseService;
        public CorreoService(IGenericRepository<Configuracion> repositorio, IFireBaseService firebaseService)
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
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
        public async Task<bool> EnviarWhatsApp(string Destino, string Mensaje, Stream archivoAdjunto, string nombreArchivo)
        {
            try
            {
                // Obtener la configuración desde la base de datos
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("Twilio"));
                Dictionary<string, string> Config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                string UrlArchivo = await _firebaseService.SubirStorage(archivoAdjunto, "carpeta_pdf", nombreArchivo);
                string telefono = Config["numerotelefono"];                


                TwilioClient.Init(Config["accountSid"], Config["authToken"]);

                var message = await MessageResource.CreateAsync(
                    from: new PhoneNumber($"whatsapp:{telefono}"), // Número de Twilio
                    to: new PhoneNumber($"whatsapp:{Destino}"),
                    body: Mensaje,
                    mediaUrl: new List<Uri> { new Uri(UrlArchivo) }
                );


                if (!string.IsNullOrEmpty(UrlArchivo))
                {
                    var respuesta = await _firebaseService.EliminarStorage("carptea_pdf", UrlArchivo);
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
