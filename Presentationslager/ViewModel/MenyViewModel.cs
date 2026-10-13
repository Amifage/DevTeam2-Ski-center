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
        public ICommand ÖppnaBokningCommand {  get; }
        public ICommand ÖppnaKundregisterCommand { get; }
        public ICommand ÖppnaUtrustningCommand { get; }
        public ICommand ÖppnaKonferenscommand {  get; }

        public ICommand ÖppnaLogiCommand { get; }
        public ICommand ÖppnaBetalstatusCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MenyViewModel()
        {
            LoggaUtCommand = new RelayCommand(LoggaUt);
            ÖppnaKundregisterCommand = new RelayCommand(ÖppnaKundregister);
            ÖppnaUtrustningCommand = new RelayCommand(ÖppnaUtrustning);
            ÖppnaSkidlektionerCommand = new RelayCommand(ÖppnaSkidlektioner);
            ÖppnaBokningCommand = new RelayCommand(ÖppnaBokning);
            ÖppnaKonferenscommand = new RelayCommand(ÖpnnaKonferens);
            ÖppnaLogiCommand = new RelayCommand(ÖpnnaLogi);
            ÖppnaBetalstatusCommand = new RelayCommand(ÖppnaBetalstatus);
        }

        public void ÖppnaKundregister(object obj)
        {
            Kundregister kundregister = new Kundregister();
            kundregister.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖppnaBetalstatus( Object obj ) 
        { 
            Betalstatus window = new Betalstatus();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖpnnaLogi(Object obj)
        {
            LogiFönster window = new LogiFönster();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖpnnaKonferens(Object obj)
        {
            KonferensFönster window = new KonferensFönster();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖppnaBokning (Object obj)
        {
            Bokning window = new Bokning();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖppnaUtrustning(object obj)
        {
            Utrustning window = new Utrustning();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void ÖppnaSkidlektioner(object obj)
        {
            Skidlektion window = new Skidlektion();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        private void LoggaUt(object obj)
        {
            Startsida window = new Startsida();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}