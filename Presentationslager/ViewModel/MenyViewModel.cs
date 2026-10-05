using Presentationslager.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class MenyViewModel : INotifyPropertyChanged
    {
        public ICommand LoggaUtCommand { get; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public MenyViewModel()
        {
            LoggaUtCommand = new RelayCommand(LoggaUt);

        }

        private void LoggaUt(object parameter)
        {
            Startsida startsida = new Startsida();
            startsida.Show();

            if (parameter is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

    }

}

