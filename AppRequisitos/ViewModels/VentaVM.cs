using CommunityToolkit.Mvvm.ComponentModel;
using AppRequisitos.Utilidades;
using CommunityToolkit.Mvvm.Input;
using AppRequisitos.Pages;
using CommunityToolkit.Mvvm.Messaging;
using AppRequisitos.DTOs;
using System.Collections.ObjectModel;
using AppRequisitos.Modelos;
using AppRequisitos.DataAccess;
using System.Drawing;

namespace AppRequisitos.ViewModels
{
    public partial class VentaVM : ObservableObject
    {
        private readonly CumplimientoDbContext _context;
        public VentaVM(CumplimientoDbContext context)
        {
            WeakReferenceMessenger.Default.Register<ProductoVentaMessage>(this, (r, m) =>
            {
                ProductoMensajeRecibido(m.Value);
            });
            _context = context;
            PropertyChanged += VentaVM_PropertyChanged;

        }

        private void VentaVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PagoCon))
            {
                if (PagoCon - Total >= 0)
                {
                    Cambio = PagoCon - Total;
                    CambioColor = System.Drawing.Color.Black;
                }
                else
                {
                    Cambio = 0;
                    CambioColor = System.Drawing.Color.Red;
                }
            }
        }

        [ObservableProperty]
        private ObservableCollection<DetalleVentaDTO> detalleVenta = new ObservableCollection<DetalleVentaDTO>();

        [ObservableProperty]
        private decimal total;

        [ObservableProperty]
        private decimal pagoCon;

        [ObservableProperty]
        private decimal cambio;

        [ObservableProperty]
        private System.Drawing.Color cambioColor;

        [RelayCommand]
        private async Task TapBuscar()
        {
            await Shell.Current.Navigation.PushModalAsync(new BuscarProductoPage(new BuscarProductoVM(new CumplimientoDbContext())));
        }

        [RelayCommand]
        private async Task TapEscanear()
        {
            await Shell.Current.Navigation.PushModalAsync(new EscanearProductoPage(new CumplimientoDbContext()));
        }

        private void ProductoMensajeRecibido(ProductoDTO result)
        {
            var encontrado = DetalleVenta.FirstOrDefault(dv => dv.Producto.IdProducto == result.IdProducto);
            if (encontrado == null)
            {
                DetalleVenta.Add(new DetalleVentaDTO
                {
                    Producto = result,
                    Cantidad = 1,
                    Total = 1 * result.Precio
                });
                MostarTotal();
            }

        }


        [RelayCommand]
        private void DisminuirEvent(DetalleVentaDTO detalle)
        {
            if (detalle.Cantidad - 1 >= 1)
            {
                detalle.Cantidad -= 1;
                detalle.Total = detalle.Producto.Precio * detalle.Cantidad;
                MostarTotal();
            }

        }

        [RelayCommand]
        private void AumentarEvent(DetalleVentaDTO detalle)
        {
            detalle.Cantidad += 1;
            detalle.Total = detalle.Producto.Precio * detalle.Cantidad;
            MostarTotal();
        }

        [RelayCommand]
        private void EliminarEvent(DetalleVentaDTO detalle)
        {
            DetalleVenta.Remove(detalle);
            MostarTotal();
        }

        public void MostarTotal()
        {
            Total = DetalleVenta.Sum(c => c.Total);
        }

        [RelayCommand]
        private async Task FinalizarVenta()
        {
            if (PagoCon == 0)
            {
                await Shell.Current.DisplayAlert("Mensaje", "Debe ingresar Fincas", "Aceptar");
                return;
            }

            if (PagoCon - Total < 0)
            {
                await Shell.Current.DisplayAlert("Mensaje", "No hay Fincas", "Aceptar");
                return;
            }

            if (DetalleVenta.Count < 1)
            {
                await Shell.Current.DisplayAlert("Mensaje", "No existen Requisitos", "Aceptar");
                return;
            }

            string nombreCliente = await Shell.Current.DisplayPromptAsync("Información del cliente", "Nombres:", accept: "Continuar", cancel: "Volver", placeholder: "(opcional)");

            try
            {
                List<DetalleVenta> detalleVentas = new List<DetalleVenta>();
                foreach (var item in DetalleVenta)
                {
                    detalleVentas.Add(new DetalleVenta
                    {
                        IdProducto = item.Producto.IdProducto,
                        Cantidad = item.Cantidad,
                        Total = item.Total
                    });
                }

                Venta venta = new Venta()
                {
                    Cliente = nombreCliente,
                    NumeroVenta = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Total = Total,
                    PagoCon = PagoCon,
                    Cambio = Cambio,
                    FechaRegistro = DateTime.Now,
                    RefDetalleVenta = detalleVentas
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await Shell.Current.DisplayAlert("Listo!", $"El número de venta '{venta.NumeroVenta}' fue generada.", "Aceptar");

                DetalleVenta.Clear();
                PagoCon = 0;
                Cambio = 0;
                MostarTotal();
            }
            catch
            {
                await Shell.Current.DisplayAlert("Error!", "No se pudo registrar la venta", "Aceptar");
            }
        }




    }
}
