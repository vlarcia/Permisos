using AppRequisitos.ViewModels;

namespace AppRequisitos.Pages;

public partial class ChecklistPage : ContentPage
{
    public ChecklistPage(ChecklistVM viewmodel)
	{
		InitializeComponent();
        BindingContext = viewmodel;
    }

}