using Presentationslager.ViewModel;
using System.Windows;

namespace Presentationslager
{
    public partial class Skidlektion : Window
    {
        public Skidlektion()
        {
            InitializeComponent();
            DataContext = new SkidlektionViewModel();
        }
    }
}