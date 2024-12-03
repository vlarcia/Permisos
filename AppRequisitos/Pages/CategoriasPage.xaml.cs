using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;


public partial class CategoriasPage : ContentPage
{
    public CategoriasPage(CategoriasVM viewmodel)
    {
        InitializeComponent();
        BindingContext = viewmodel;
    }

}