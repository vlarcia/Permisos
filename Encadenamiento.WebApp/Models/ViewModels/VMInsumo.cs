namespace Permisos.WebApp.Models.ViewModels
{
    public class VMInsumo
    {
      

        public int Coditem { get; set; }

        public string Descripcion { get; set; } = null!;

         public string Umedida { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public string? Ubicacion { get; set; }

        public int? Codprovee { get; set; }

        public decimal? Reorden { get; set; }

        public decimal? Costo { get; set; }

        public decimal? Precio { get; set; }

        public decimal? Impuesto { get; set; }

        public decimal? Existencia { get; set; }

        public string? Cuentaventa { get; set; }

        public string? Cuentacosto { get; set; }

        public string? Cuentainven { get; set; }

        public byte[]? Imagen { get; set; }

        public decimal? Factor { get; set; }

        public bool? Activo { get; set; }
    }
}
