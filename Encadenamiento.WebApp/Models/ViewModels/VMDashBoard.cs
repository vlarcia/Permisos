namespace Permisos.WebApp.Models.ViewModels
{
    public class VMDashBoard
    {
        public string? TotalPermisos {  get; set; }
        public string? TotalDestinatarios { get; set; }           
        public string? AlertasUltimoMes { get; set; }
        public string? VencimientosMes {  get; set; }
        public int TotalPermisosVencidosNoTramite { get; set; }


        public List<VMListaDashBoard> Renovaciones { get; set; }
        public List<VMListaDashBoard> Vencimientos { get; set; }        


    }
}
