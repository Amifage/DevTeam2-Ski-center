using Presentationslager.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class MenyViewModel : INotifyPropertyChanged
    {
        public ICommand LoggaUtCommand { get; }
        public ICommand ÖppnaSkidlektionerCommand { get; }
        public ICommand ÖppnaKundregisterCommand { get; }
        public ICommand ÖppnaUtrustningCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MenyViewModel()
        {
            LoggaUtCommand = new RelayCommand(LoggaUt);
            ÖppnaKundregisterCommand = new RelayCommand(ÖppnaKundregister);
            ÖppnaUtrustningCommand = new RelayCommand(ÖppnaUtrustning);
            ÖppnaSkidlektionerCommand = new RelayCommand(ÖppnaSkidlektioner);
        }

        public void ÖppnaKundregister(object? obj)
        {
            Kundregister kundregister = new Kundregister();
            kundregister.Show();
        }

        public void ÖppnaUtrustning(object? obj)
        {
            Utrustning utrustning = new Utrustning();
            utrustning.Show();
        }

        public void ÖppnaSkidlektioner(object obj)
        {
            Skidlektion skidlektion = new Skidlektion();
            skidlektion.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        private void LoggaUt(object? parameter)
        {
            Startsida startsida = new Startsida();
            startsida.Show();
        }


        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}