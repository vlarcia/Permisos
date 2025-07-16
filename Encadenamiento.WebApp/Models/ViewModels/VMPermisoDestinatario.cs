namespace Permisos.WebApp.Models.ViewModels
{
    public class VMPermisoDestinatario
    {
        public int IdPermisoDestinatario { get; set; }
        public int IdPermiso { get; set; }
        public int IdDestinatario { get; set; }

        public string? NombrePermiso { get; set; }
        public string? Institucion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public DateTime? FechaAsignacion { get; set; }

    }

}