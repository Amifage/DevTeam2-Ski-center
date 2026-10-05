
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
            OpenMenuCommand = new RelayCommand(ExecuteOpenMenu);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void ExecuteOpenMenu(object parameter)
        {
            Meny menyFönster = new Meny();
            menyFönster.Show();

            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}