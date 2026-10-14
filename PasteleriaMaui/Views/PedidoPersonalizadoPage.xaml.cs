using Microsoft.Maui.Controls;

namespace PasteleriaMaui.Views
{
    public partial class PedidoPersonalizadoPage : ContentPage
    {
        public PedidoPersonalizadoPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.PedidoPersonalizadoViewModel();
        }

        private async void OnExpandImageTapped(object sender, TappedEventArgs e)
        {
            var url = e.Parameter as string;
            if (!string.IsNullOrWhiteSpace(url))
            {
                await Navigation.PushModalAsync(new ImagenLightboxPage(url), true);
            }
        }
    }
}
