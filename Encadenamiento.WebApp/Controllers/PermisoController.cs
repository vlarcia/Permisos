using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using Permisos.WebApp.Models.ViewModels;
using Permisos.WebApp.Utilidades.Response;
using Business.Interfaces;
using Entity;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Business.Implementacion;
using Microsoft.AspNetCore.Authorization;


namespace Permisos.WebApp.Controllers
{
    public class PermisoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPermisoService _permisoService;
        public PermisoController(IPermisoService permisoService, IMapper mapper)
        {
            _permisoService = permisoService;
            _mapper = mapper;
        }

        
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaPermisos()
        {
            var idAreaClaim = User.Claims.FirstOrDefault(c => c.Type == "IdArea")?.Value;
            int idArea = 0;
            if (!string.IsNullOrEmpty(idAreaClaim))
            {
                int.TryParse(idAreaClaim, out idArea);
            }
            // Obtener todos los permisos
            var permisos = await _permisoService.ListaPorArea(idArea);

            // Filtrar los permisos que vencen en menos de 6 meses
            var fechaActual = DateTime.Now;
            var fechaLimite = fechaActual.AddMonths(6);

            var permisosFiltrados = permisos.Where(p =>
                p.EstadoPermiso != "EN TRÁMITE" &&
                p.FechaVencimiento <= fechaLimite
            ).ToList();


            // Mapear a VMPermiso
            List<VMPermiso> vmPermisoLista = _mapper.Map<List<VMPermiso>>(permisosFiltrados);

            return StatusCode(StatusCodes.Status200OK, vmPermisoLista);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var idAreaClaim = User.Claims.FirstOrDefault(c => c.Type == "IdArea")?.Value;
            int idArea = 0;
            if (!string.IsNullOrEmpty(idAreaClaim))
            {
                int.TryParse(idAreaClaim, out idArea);
            }

            List<VMPermiso> vmPermisoLista = _mapper.Map<List<VMPermiso>>(await _permisoService.ListaPorArea(idArea));
            return StatusCode(StatusCodes.Status200OK, new { data = vmPermisoLista });
        }
        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile archivoEvidencia, [FromForm] IFormFile archivoEvidencia2, [FromForm] IFormFile archivoEvidencia3, [FromForm] string modelo)
        {
            GenericResponse<VMPermiso> gResponse = new GenericResponse<VMPermiso>();
            try

            {
                VMPermiso vmPermiso = JsonConvert.DeserializeObject<VMPermiso>(modelo);

                string nombreArchivo = ""; string nombreArchivo2 = "", nombreArchivo3 = "";
                Stream stream1 = null; Stream stream2 = null, stream3 = null;

                if (archivoEvidencia != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    
                    nombreArchivo = string.Concat(vmPermiso.IdPermiso.ToString(),"_", vmPermiso.NombreEvidencia);
                    stream1 = archivoEvidencia.OpenReadStream();
                }


                if (archivoEvidencia2 != null)
                {
                    nombreArchivo2 = $"{vmPermiso.IdPermiso}_2_{vmPermiso.NombreEvidencia2}";
                    stream2 = archivoEvidencia2.OpenReadStream();
                }

                if (archivoEvidencia3 != null)
                {
                    nombreArchivo3 = $"{vmPermiso.IdPermiso}_3_{vmPermiso.NombreEvidencia3}";
                    stream3 = archivoEvidencia3.OpenReadStream();
                }

                TbPermiso permiso_creado = await _permisoService.Crear(
                    _mapper.Map<TbPermiso>(vmPermiso),
                    stream1, nombreArchivo,
                    stream2, nombreArchivo2,
                    stream3, nombreArchivo3
                );

                //TbPermiso permiso_creado = await _permisoService.Crear(_mapper.Map<TbPermiso>(vmPermiso), archivoStream, nombreArchivo);
                vmPermiso = _mapper.Map<VMPermiso>(permiso_creado);
                gResponse.Estado = true;
                gResponse.Objeto = vmPermiso;

            }
            catch (Exception ex)
            {

                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] IFormFile archivoEvidencia, [FromForm] IFormFile archivoEvidencia2, 
                                         [FromForm] IFormFile archivoEvidencia3, [FromForm] string modelo)  
        {
            GenericResponse<VMPermiso> gResponse = new GenericResponse<VMPermiso>();
            try
            {
                VMPermiso vmPermiso = JsonConvert.DeserializeObject<VMPermiso>(modelo);

                string nombreArchivo = ""; string nombreArchivo2 = "", nombreArchivo3 = "";
                Stream stream1 = null; Stream stream2 = null, stream3 = null;

                if (archivoEvidencia != null)
                {
                    //string nombre_en_codigo = Guid.NewGuid().ToString("N");

                    nombreArchivo = string.Concat(vmPermiso.IdPermiso.ToString(), "_", vmPermiso.NombreEvidencia);
                    stream1 = archivoEvidencia.OpenReadStream();
                }


                if (archivoEvidencia2 != null)
                {
                    nombreArchivo2 = $"{vmPermiso.IdPermiso}_2_{vmPermiso.NombreEvidencia2}";
                    stream2 = archivoEvidencia2.OpenReadStream();
                }

                if (archivoEvidencia3 != null)
                {
                    nombreArchivo3 = $"{vmPermiso.IdPermiso}_3_{vmPermiso.NombreEvidencia3}";
                    stream3 = archivoEvidencia3.OpenReadStream();
                }

                TbPermiso permiso_editado = await _permisoService.Editar(
                    _mapper.Map<TbPermiso>(vmPermiso),
                        stream1, nombreArchivo,
                        stream2, nombreArchivo2,
                        stream3, nombreArchivo3,
                        vmPermiso.EliminarEvidencia,
                        vmPermiso.EliminarEvidencia2,
                        vmPermiso.EliminarEvidencia3);

                vmPermiso = _mapper.Map<VMPermiso>(permiso_editado);
                gResponse.Estado = true;
                gResponse.Objeto = vmPermiso;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);

        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int idPermiso)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _permisoService.Eliminar(idPermiso);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
