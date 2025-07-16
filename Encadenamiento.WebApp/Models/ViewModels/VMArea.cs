namespace Permisos.WebApp.Models.ViewModels
{
    public class VMArea
    {
        public int IdArea { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
