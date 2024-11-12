namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMPDFRevision
    {
        public List<VMRevisiones>? revision {  get; set; }
        public VMNegocio? negocio { get; set; }
        public VMRevisions? generales { get; set; }
    }
}
