namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMDashBoard
    {
        public string? TotalFincas {  get; set; }
        public string? TotalPlanes { get; set; }           
        public string? TotalRevisiones { get; set; }
        public string? TotalVisitas {  get; set; }
        public string? TotalActividades { get; set; }

        public List<VMListaDashBoard> ActividadesCompletas { get; set; }
        public List<VMListaDashBoard> FincasVisitadas { get; set; }
        public List<VMCumplimiento> CumplimientoGlobal { get; set; }


    }
}
