
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

        
    }
}