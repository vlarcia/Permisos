using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class HistoriaVentaPage : ContentPage
{
	public HistoriaVentaPage(HistorialVentaVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}