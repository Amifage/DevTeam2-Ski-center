
using Presentationslager.Command;
using Servicelager;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class StartsidaViewModel : INotifyPropertyChanged
    {
        public ICommand LoggaInCommand { get; }

        public StartsidaViewModel()
        {
            LoggaInCommand = new RelayCommand(LoggaIn);
        }

        private string _epost = string.Empty;
        public string Epost
        {
            get { return _epost; }
            set
            {
                _epost = value;
                OnPropertyChanged(nameof(Epost));
            }
        }

        private void LoggaIn(object obj)
        {
            if (obj is not Startsida fönster)
            {
                return;
            }

            // PasswordBox kan inte bindas, så lösenordet hämtas direkt från fönstret
            string lösenord = fönster.txtLösenord.Password;

            if (string.IsNullOrWhiteSpace(Epost) || string.IsNullOrWhiteSpace(lösenord))
            {
                MessageBox.Show("Ange både e-post och lösenord.",
                                "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool inloggad;

            try
            {
                inloggad = InloggningController.LoggaIn(Epost.Trim(), lösenord);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte ansluta till databasen:\n" + (ex.InnerException?.Message ?? ex.Message));
                return;
            }

            if (!inloggad)
            {
                MessageBox.Show("Felaktig e-post eller lösenord.",
                                "Inloggning misslyckades", MessageBoxButton.OK, MessageBoxImage.Error);
                fönster.txtLösenord.Clear();
                return;
            }

            Meny window = new Meny();
            window.Show();
            fönster.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}