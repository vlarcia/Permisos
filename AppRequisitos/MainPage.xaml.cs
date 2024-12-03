using AppRequisitos.ViewModels;

namespace AppRequisitos
{
    public partial class MainPage : ContentPage
    {      
        public MainPage(MainVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }        
    }
}