using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using Data.Interfaces;
using Entity;

using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Security;

namespace Business.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<TbUsuario> _repositorio;
        private readonly IFireBaseService _firebaseService;
        private readonly IUtilidadesService _utilidadesService; 
        private readonly ICorreoService _correoService;
        public UsuarioService(
            IGenericRepository<TbUsuario> repositorio,
            IFireBaseService firebaseService,
            IUtilidadesService utilidadesService,
            ICorreoService correoService
            )
        {
            _repositorio = repositorio;
            _firebaseService = firebaseService;
            _utilidadesService= utilidadesService;
            _correoService = correoService; 

        }
        public async Task<List<TbUsuario>> Lista()
        {
            IQueryable<TbUsuario> query = _repositorio.Consultar();
            return await query.Include(r => r.IdRolNavigation).Include(a=> a.IdAreaNavigation).ToListAsync();    
        }
        public async Task<TbUsuario> Crear(TbUsuario entidad, Stream foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "")
        {
            TbUsuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);
            if (usuario_existe != null)
                throw new TaskCanceledException("El usuario ya existe");
            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave= _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = NombreFoto;
                if (foto != null)
                {
                    string UrlFoto = await _firebaseService.SubirStorage(foto, "carpeta_usuario", NombreFoto);
                    entidad.UrlFoto=UrlFoto;
                }
                TbUsuario usuario_creado = await _repositorio.Crear(entidad);
                if (usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el Usuario!");

                if (UrlPlantillaCorreo != "")
                {
                    UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo).Replace("[clave]", clave_generada);
                    string htmlCorreo = "";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;
                            if (response.CharacterSet != null)
                                readerStream = new StreamReader(dataStream);
                            else
                                readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }
                    if (htmlCorreo != null)
                        await _correoService.EnviarCorreo(usuario_creado.Correo, "Cuenta Creada", htmlCorreo,null,null);
                }
                IQueryable<TbUsuario> query=_repositorio.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                return await query.Include(r=>r.IdRolNavigation).FirstAsync();

            }
            catch(Exception ex) 
            {
                throw;
            }
        }

        public async Task<TbUsuario> Editar(TbUsuario entidad, Stream Foto = null, string NombreFoto = "")
        {
            TbUsuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);
            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");
            try
            {
                IQueryable<TbUsuario> queryUsuario = _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                TbUsuario usuario_editar = queryUsuario.First();
                usuario_editar.Nombre=entidad.Nombre;
                usuario_editar.Correo=entidad.Correo;   
                usuario_editar.Telefono=entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo=entidad.EsActivo;
                usuario_editar.IdArea = entidad.IdArea;

                if (usuario_editar.NombreFoto == "")
                    usuario_editar.NombreFoto = NombreFoto;

                if (Foto != null)
                {
                    string urlFoto = await _firebaseService.SubirStorage(Foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuario_editar);
                if(!respuesta)
                    throw new TaskCanceledException("No se pudo actualizar el usuario.  Revise !");
                return await queryUsuario.Include(r=> r.IdRolNavigation).FirstAsync();
                
            }
            catch 
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int IdUsuario)
        {
            try
            {
                TbUsuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe!");
                string nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);
                if (respuesta)
                    await _firebaseService.EliminarStorage("carpeta_usuario", nombreFoto);
                return true;
            }
            catch
            {
                throw;
            }
        }       

        public async Task<TbUsuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);
            TbUsuario usuario_encontardo = await _repositorio.Obtener(u=>u.Correo == correo && u.Clave==clave_encriptada);
            return usuario_encontardo;  
        }

        public async Task<TbUsuario> ObtenerPorId(int IdUsuario)
        {
            IQueryable<TbUsuario> query = _repositorio.Consultar(u => u.IdUsuario == IdUsuario);                
            return await query.Include(r => r.IdRolNavigation).FirstOrDefaultAsync();

        }
        public async Task<bool> GuardarPerfil(TbUsuario entidad)
        {
            try 
            {
                TbUsuario usuario_encontrado=await _repositorio.Obtener(u=> u.IdUsuario==entidad.IdUsuario);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe!");
                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;
                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch 
            {
                throw;
            }

        }

        public async Task<bool> CambiarClave(int IdUsuario, string ClaveActual, string ClaveNueva)
        {
            try 
            {
                TbUsuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == IdUsuario);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe!");

                if (usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(ClaveActual))               
                    throw new TaskCanceledException("Clave actual errónea!");

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(ClaveNueva);
                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch
            {
                throw;
            }

        }
        public async Task<bool> RestablecerClave(string Correo, string UrlPlantillaCorreo)
        {
            try
            {
                TbUsuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo == Correo);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No existe usuario asociado a ese correo!");

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);


                UrlPlantillaCorreo = UrlPlantillaCorreo.Replace("[clave]", clave_generada);

                string htmlCorreo = "";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UrlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;
                        if (response.CharacterSet != null)
                            readerStream = new StreamReader(dataStream);
                        else
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        htmlCorreo = readerStream.ReadToEnd();
                        response.Close();
                        readerStream.Close();
                    }
                }
                bool correo_enviado = false;
                if (htmlCorreo != "")
                    correo_enviado = await _correoService.EnviarCorreo(Correo, "Contraseña Restablecida", htmlCorreo, null, null);
                if(!correo_enviado)
                    throw new TaskCanceledException("Existen problemas con el envío de correo.  Intente más tarde!");
                
                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;

            }
            catch
            {
                throw;
            }
        }
    }
}
