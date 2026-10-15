using Presentationslager.Command;
using Servicelager;
using Entitetslager; 
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class LogiViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly LogiController logiController;

        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraStatusCommand { get; }
        public ICommand TillbakaCommand { get; set; }


        private ObservableCollection<Entitetslager.Logi> _logiLista;
        public ObservableCollection<Entitetslager.Logi> LogiLista
        {
            get { return _logiLista; }
            set { _logiLista = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _artikelTyper;
        public ObservableCollection<string> ArtikelTyper
        {
            get { return _artikelTyper; }
            set { _artikelTyper = value; OnPropertyChanged(); }
        }

        private string _valdArtikelTyp;
        public string ValdArtikelTyp
        {
            get { return _valdArtikelTyp; }
            set { _valdArtikelTyp = value; OnPropertyChanged(); }
        }
        private Entitetslager.Logi _valdLogi;
        public Entitetslager.Logi ValdLogi
        {
            get { return _valdLogi; }
            set
            {
                _valdLogi = value;
                OnPropertyChanged();
            }
        }

        private string _sokLogiNummer;
        public string SokLogiNummer
        {
            get { return _sokLogiNummer; }
            set { _sokLogiNummer = value; OnPropertyChanged(); }
        }

        private string _sokStatus;
        public string SokStatus
        {
            get { return _sokStatus; }
            set { _sokStatus = value; OnPropertyChanged(); }

        }

        private ObservableCollection<string> _statusAlternativ;
        public ObservableCollection<string> StatusAlternativ
        {
            get { return _statusAlternativ; }
            set { _statusAlternativ = value; OnPropertyChanged(); }
        }

        public LogiViewModel()
        {
            logiController = new LogiController();

            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);

            UppdateraStatusCommand = new RelayCommand(UppdateraStatus, CanUppdateraStatus);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            LogiLista = new ObservableCollection<Entitetslager.Logi>();
            ArtikelTyper = new ObservableCollection<string>();

            StatusAlternativ = new ObservableCollection<string>
           {      
             "Tillgänglig",
             "Ur bruk"
            };

            LaddaData();
        }
        
            private void LaddaData()
            {
                var allLogi = logiController.HamtaAllaLogi(); 

                if (LogiLista == null)
                {
                    LogiLista = new ObservableCollection<Entitetslager.Logi>();
                }
                LogiLista.Clear();

                if (allLogi != null)
                {
                    foreach (Entitetslager.Logi item in allLogi)
                    {
                        LogiLista.Add(item);
                    }
                }

                var typer = logiController.HamtaAllaArtikelTyper(); // Se till att denna metod finns i LogiController

                if (ArtikelTyper == null)
                {
                    ArtikelTyper = new ObservableCollection<string>();
                }
                ArtikelTyper.Clear();

            if (typer != null)
            {
                var filtreradeTyper = typer
                    .Where(t => t != null && !string.IsNullOrWhiteSpace(t.TypNamn))
                    .Select(t => t.TypNamn)
                    .Where(namn =>
                        namn.Contains("LGH.I", StringComparison.OrdinalIgnoreCase) ||
                        namn.Contains("LGH.II", StringComparison.OrdinalIgnoreCase) ||
                        namn.Contains("Camp", StringComparison.OrdinalIgnoreCase) );                        

                    foreach (var typ in filtreradeTyper)
                    {
                        ArtikelTyper.Add(typ);
                    }
                }
            }

        private void UtforSokning(object obj)
        {
            var resultat = logiController.SokLogi(
                SokLogiNummer,
                SokStatus,
                ValdArtikelTyp
            );

            LogiLista.Clear();
            if (resultat != null)
            {
                foreach (Entitetslager.Logi item in resultat)
                {
                    LogiLista.Add(item);
                }
            }
        }

        private void RensaFalt(object obj)
        {
            SokLogiNummer = string.Empty;
            SokStatus = string.Empty;
            LaddaData(); 
        }

        private bool CanUppdateraStatus(object obj)
        {
            return ValdLogi != null;
        }

        private void UppdateraStatus(object obj)
        {
            if (ValdLogi != null)
            {
                if (ValdLogi.Status == "Tillgänglig")
                {
                    ValdLogi.Status = "Ur bruk";
                }
                else
                {
                    ValdLogi.Status = "Tillgänglig";
                }

                logiController.UppdateraLogi(ValdLogi);

                MessageBox.Show($"Status för {ValdLogi.LogiNummer} har ändrats till: {ValdLogi.Status}",
                                "Status uppdaterad",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                LaddaData();
            }
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny menyFonster = new Meny();
            menyFonster.Show();

            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
