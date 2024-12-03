using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class FincasPage : ContentPage
{
    public FincasPage(FincasVM viewmodel)
	{
		InitializeComponent();
        BindingContext = viewmodel;
    }

}