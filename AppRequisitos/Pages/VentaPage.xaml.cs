using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class VentaPage : ContentPage
{
	public VentaPage(VentaVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}