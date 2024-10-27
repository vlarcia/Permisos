namespace Encadenamiento.WebApp.Models.ViewModels
{
    public class VMReporteCompra
    {
        

        
        public DateTime Fechafact { get; set; }

        public long Factura { get; set; }
                
        public string? NombreProveedor { get; set; }
        
        public decimal? Valorcompra { get; set; }

        public decimal? Valorimpuesto { get; set; }

        public decimal? Valordescuento { get; set; }

        public int Coditem { get; set; }
        public string? insumo { get; set; }


        public decimal? Cantidad { get; set; }

        public decimal? Monto { get; set; }
            

        public decimal? Unitario { get; set; }

    }
}
