using Entity;

namespace Permisos.WebApp.Models.ViewModels
{
    public class VMUsuario
    {

        public int IdUsuario { get; set; }

        public string? Nombre { get; set; }

        public string? Correo { get; set; }

        public string? Telefono { get; set; }

        public int? IdRol { get; set; }
        public string? NombreRol { get; set; }
        public string? UrlFoto { get; set; }
        public int? EsActivo { get; set; }
        public int IdArea { get; set; }
        public string NombreArea { get; set; } = null!;
    }}
