
using Presentationslager.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class StartsidaViewModel : INotifyPropertyChanged

    {
        public ICommand OpenMenuCommand { get; }

        public StartsidaViewModel()
        {
            OpenMenuCommand = new RelayCommand(OppnaMeny);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OppnaMeny(object obj)
        {
            Meny window = new Meny();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }
    }
}