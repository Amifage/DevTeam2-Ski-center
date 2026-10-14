using Presentationslager.ViewModel;
using Servicelager;
using System.Windows;

namespace Presentationslager
{
    public partial class Skidlektion : Window
    {
        public Skidlektion()
        {
            InitializeComponent();
            DataContext = new SkidlektionViewModel(new SkidlektionController());
        }
    }
}