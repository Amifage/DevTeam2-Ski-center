using System.Windows;
using Presentationslager.ViewModel;

namespace Presentationslager
{
    public partial class Bokning : Window
    {
        public Bokning()
        {
            InitializeComponent();

            BokningViewModel viewModel = new BokningViewModel();

            viewModel.VisaMeddelande += VisaMeddelande;

            DataContext = viewModel;
        }

        private void VisaMeddelande(string meddelande)
        {
            MessageBox.Show(meddelande);
        }

        private void StängFönster()
        {
            Close();
        }
    }
}