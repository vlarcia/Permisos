using AppRequisitos.ViewModels;
namespace AppRequisitos.Pages;

public partial class ChecklistDetallePage : ContentPage
{
    private readonly ChecklistDetalleVM _viewModel;
    private readonly int _idRequisito;    
       
    public ChecklistDetallePage(ChecklistDetalleVM vm, int idRequisito)
    {
        InitializeComponent();
        BindingContext = vm;
        _viewModel = vm;
        _idRequisito = idRequisito;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.CargarDetalles(); // Cargar detalles del requisito
    }
}
