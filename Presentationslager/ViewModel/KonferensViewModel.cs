using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Entitetslager;
using Servicelager;
using Presentationslager.Command;

namespace Presentationslager.ViewModel
{
    public class KonferensViewModel : INotifyPropertyChanged
    {
        private readonly KonferensController _konferensController;

        private ObservableCollection<Entitetslager.Konferens> _konferensLista;
        public ObservableCollection<Entitetslager.Konferens> KonferensLista
        {
            get => _konferensLista;
            set { _konferensLista = value; OnPropertyChanged(); }
        }

        private Entitetslager.Konferens _valdKonferensrum;
        public Entitetslager.Konferens ValdKonferensrum
        {
            get => _valdKonferensrum;
            set { _valdKonferensrum = value; OnPropertyChanged(); }
        }

        private string _sokRumsNummer;
        public string SokRumsNummer
        {
            get => _sokRumsNummer;
            set { _sokRumsNummer = value; OnPropertyChanged(); }
        }

        private string _sokStatus;
        public string SokStatus
        {
            get => _sokStatus;
            set { _sokStatus = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _rumsTyper;
        public ObservableCollection<string> RumsTyper
        {
            get => _rumsTyper;
            set { _rumsTyper = value; OnPropertyChanged(); }
        }

        private string _valdRumsTyp;
        public string ValdRumsTyp
        {
            get => _valdRumsTyp;
            set { _valdRumsTyp = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _statusAlternativ;
        public ObservableCollection<string> StatusAlternativ
        {
            get { return _statusAlternativ; }
            set { _statusAlternativ = value; OnPropertyChanged(); }
        }

        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraStatusCommand { get; }
        public ICommand TillbakaCommand { get; set; }

        public KonferensViewModel()
        {
            _konferensController = new KonferensController();

            KonferensLista = new ObservableCollection<Entitetslager.Konferens>();

            RumsTyper = new ObservableCollection<string>
            {
                "Liten (max 20)",
                "Stor (max 30)"
            };

            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);
            UppdateraStatusCommand = new RelayCommand(UppdateraStatus, CanUppdateraStatus);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);

            StatusAlternativ = new ObservableCollection<string>
           {
             "Tillgänglig",
             "Ur bruk"
            };

            LaddaData();
        }

        private void LaddaData()
        {
            var konferenser = _konferensController.HamtaAllaKonferenser();
            KonferensLista.Clear();
            if (konferenser != null)
            {
                foreach (Entitetslager.Konferens item in konferenser)
                {
                    KonferensLista.Add(item);
                }
            }
        }

        private void UtforSokning(object obj)
        {
            var resultat = _konferensController.SokKonferens(
                SokRumsNummer,
                SokStatus,
                ValdRumsTyp
            );

            KonferensLista.Clear();
            if (resultat != null)
            {
                foreach (Entitetslager.Konferens item in resultat)
                {
                    KonferensLista.Add(item);
                }
            }
        }

        private void RensaFalt(object obj)
        {
            SokRumsNummer = string.Empty;
            SokStatus = string.Empty;
            ValdRumsTyp = null;

            LaddaData();
        }

        private bool CanUppdateraStatus(object obj)
        {
            return ValdKonferensrum != null;
        }

        private void UppdateraStatus(object obj)
        {
            if (ValdKonferensrum == null) return;

            var vald = ValdKonferensrum;

            string nyStatus = (vald.Status != null && vald.Status.Equals("Tillgänglig", StringComparison.OrdinalIgnoreCase))
                ? "Ur bruk"
                : "Tillgänglig";

            bool lyckades = _konferensController.UppdateraKonferensStatus(vald.KonferensNummer, nyStatus);

            if (lyckades)
            {
                vald.Status = nyStatus;
                vald.SenastUppdaterad = DateTime.Now;

                int index = KonferensLista.IndexOf(vald);
                if (index >= 0)
                {
                    KonferensLista[index] = vald;
                }

                ValdKonferensrum = vald;

                MessageBox.Show($"Status för rum {vald.KonferensNummer} uppdaterades till: {nyStatus}",
                                "Status uppdaterad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Det gick inte att uppdatera statusen för det valda konferensrummet.",
                                "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}