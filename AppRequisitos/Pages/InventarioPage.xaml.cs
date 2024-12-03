using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class InventarioPage : ContentPage
{
	public InventarioPage(InventarioVM viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}