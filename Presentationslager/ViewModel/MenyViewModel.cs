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
        public ICommand OppnaSkidlektionerCommand { get; }
        public ICommand OppnaBokningCommand {  get; }
        public ICommand OppnaKundregisterCommand { get; }
        public ICommand OppnaUtrustningCommand { get; }
        public ICommand OppnaKonferenscommand {  get; }

        public ICommand OppnaLogiCommand { get; }
        public ICommand OppnaBetalstatusCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MenyViewModel()
        {
            LoggaUtCommand = new RelayCommand(LoggaUt);
            OppnaKundregisterCommand = new RelayCommand(OppnaKundregister);
            OppnaUtrustningCommand = new RelayCommand(OppnaUtrustning);
            OppnaSkidlektionerCommand = new RelayCommand(OppnaSkidlektioner);
            OppnaBokningCommand = new RelayCommand(OppnaBokning);
            OppnaKonferenscommand = new RelayCommand(OpnnaKonferens);
            OppnaLogiCommand = new RelayCommand(OppnaLogi);
            OppnaBetalstatusCommand = new RelayCommand(OppnaBetalstatus);
        }

        public void OppnaKundregister(object obj)
        {
            Kundregister window = new Kundregister();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OppnaBetalstatus( Object obj ) 
        { 
            Betalstatus window = new Betalstatus();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OppnaLogi(Object obj)
        {
            LogiFönster window = new LogiFönster();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OpnnaKonferens(Object obj)
        {
            KonferensFönster window = new KonferensFönster();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OppnaBokning (Object obj)
        {
            Bokning window = new Bokning();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OppnaUtrustning(object obj)
        {
            Utrustning window = new Utrustning();
            window.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        public void OppnaSkidlektioner(object obj)
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