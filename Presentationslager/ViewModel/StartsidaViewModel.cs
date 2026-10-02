using System.Windows;
using System.Windows.Input;
using Presentationslager.Command;

namespace Presentationslager.ViewModel
{
    public class StartsidaViewModel
    {
        // Kommandot som knappen binder till i XAML
        public ICommand OpenMenuCommand { get; }

        public StartsidaViewModel()
        {
            // Kopplar kommandot till metoden som körs vid klick
            OpenMenuCommand = new RelayCommand(ExecuteOpenMenu);
        }

        private void ExecuteOpenMenu(object parameter)
        {
            // 1. Skapa och visa menyfönstret
            Meny menyFönster = new Meny();
            menyFönster.Show();

            // 2. Stäng det nuvarande fönstret (Startsidan) på ett säkert sätt via CommandParameter
            if (parameter is Window window)
            {
                window.Close();
            }
        }
    }
}