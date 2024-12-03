using System.Windows.Input;

namespace AppRequisitos.Pages;

public partial class DeviceAuthPage : ContentPage
{
    public string AndroidId { get; }

    public DeviceAuthPage(string androidId)
    {
        InitializeComponent();
        AndroidId = androidId;
        BindingContext = this;
    }

    public ICommand CopyCommand => new Command(() =>
    {
        Clipboard.SetTextAsync(AndroidId);
        DisplayAlert("Copiado", "El Android ID fue copiado al portapapeles.", "OK");
    });

    public ICommand VolverCommand => new Command(() =>
    {
        Application.Current.Quit(); // O regresa a otra pantalla si aplica.
    });
}