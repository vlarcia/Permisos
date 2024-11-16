using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
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
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Twilio.TwiML.Messaging;



namespace Business.Implementacion
{
    public class CorreoService : ICorreoService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;
        private readonly IFireBaseService _firebaseService;
        private IWebDriver? driver;

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
        public async Task<bool> MensajeWhatsApp(string Destino, string Mensaje, Stream archivoAdjunto, string nombreArchivo)
        {
            try
            {
                string UrlArchivo = await _firebaseService.SubirStorage(archivoAdjunto, "carpeta_pdf", nombreArchivo);
                // Busca el campo de búsqueda y escribe el número de teléfono

                IniciarWhatsAppWeb();

                IWebElement searchBox = driver.FindElement(By.XPath("//div[@contenteditable='true'][@data-tab='3']"));
                searchBox.SendKeys(Destino);
                searchBox.SendKeys(Keys.Enter);

                // Esperar a que la conversación se cargue
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.XPath("//span[@title='" + Destino + "']")));

                // Esperar el campo para escribir el mensaje
                IWebElement messageBox = driver.FindElement(By.XPath("//div[@contenteditable='true'][@data-tab='1']"));
                messageBox.SendKeys(Mensaje);

                // Enviar el mensaje
                messageBox.SendKeys(Keys.Enter);

                // Si hay archivo adjunto, enviar el archivo
                if (!string.IsNullOrEmpty(UrlArchivo))
                {
                    // Hacer click en el ícono de adjuntar archivo
                    IWebElement attachButton = driver.FindElement(By.XPath("//span[@data-icon='clip']"));
                    attachButton.Click();

                    // Esperar y seleccionar el archivo
                    IWebElement fileInput = driver.FindElement(By.XPath("//input[@type='file']"));
                    fileInput.SendKeys(UrlArchivo);

                    // Esperar a que el archivo se cargue
                    wait.Until(d => d.FindElement(By.XPath("//span[@data-icon='send']"))).Click();
                }

                Console.WriteLine("Mensaje enviado correctamente.");



                if (!string.IsNullOrEmpty(UrlArchivo))
                {
                    var respuesta = await _firebaseService.EliminarStorage("carptea_pdf", nombreArchivo);
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
        public void IniciarWhatsAppWeb()
    {
        ChromeOptions options = new ChromeOptions();

        // Agregar argumentos para evitar problemas en entornos sin interfaz gráfica (opcional)
        options.AddArguments("--headless"); // Si es necesario, no mostrar interfaz gráfica
        options.AddArguments("--disable-gpu"); // Desactiva la aceleración por hardware
        options.AddArguments("--no-sandbox"); // Desactiva el sandbox (útil en entornos controlados)
        options.AddArguments("--disable-dev-shm-usage"); // Evita el uso compartido de memoria


            // Ruta a tu chromedriver.exe
            string chromedriverPath = Path.Combine(Directory.GetCurrentDirectory(), "drivers");


            driver = new ChromeDriver(chromedriverPath, options);

        // Verifica si ya estás en la página de WhatsApp Web
        try
        {
            // Comprueba si el elemento de búsqueda o un contacto está presente, lo que indica que la sesión está activa
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElement(By.XPath("//div[@title='Nuevo chat']")));

            Console.WriteLine("Sesión de WhatsApp Web ya está activa.");
        }
        catch (NoSuchElementException)
        {
            // Si no se encuentra el elemento de "Nuevo chat", significa que no estamos en la página de WhatsApp Web
            Console.WriteLine("No se detectó sesión activa de WhatsApp Web. Asegúrate de que WhatsApp Web esté abierto.");
        }
    }


    }
}
