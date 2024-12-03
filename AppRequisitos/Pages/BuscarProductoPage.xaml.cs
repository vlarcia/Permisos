using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class BuscarProductoPage : ContentPage
{
	public BuscarProductoPage(BuscarProductoVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}