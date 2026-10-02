
using Presentationslager.ViewModel;
using System.Windows;


namespace Presentationslager
{
    public partial class Startsida : Window
    {
        public Startsida()
        {
            InitializeComponent();
            DataContext = new StartsidaViewModel();
        }

        // Lägg till denna metod så att den matchar händelsen i XAML
        private void BtnLoggaIn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Inloggningsknappen klickad!");
        }
    }
}