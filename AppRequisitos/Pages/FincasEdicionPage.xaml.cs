using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class FincasEdicionPage : ContentPage
{
	private readonly FincasEdicionVM viewModel;
    private readonly int _idFinca;
	public FincasEdicionPage(FincasEdicionVM vm, int idFinca)
	{
		InitializeComponent();
        BindingContext = vm;
        viewModel = vm;
        _idFinca = idFinca;

    }

    protected override void OnAppearing()
    {
        
        viewModel.Inicio(_idFinca);
    }

    //protected override void OnDisappearing()
    //{
    //    viewModel.Desuscribir();
    //}
}