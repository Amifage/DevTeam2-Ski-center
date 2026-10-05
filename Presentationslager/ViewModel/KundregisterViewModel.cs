using System.Collections.ObjectModel;
using System.Windows.Input;
using Entitetslager;
using Servicelager;
using Presentationslager.Command;
using System.ComponentModel;
using System.Windows;

namespace Presentationslager.ViewModel
{
    public class KundregisterViewModel : INotifyPropertyChanged // Antar att du har en BaseViewModel för INotifyPropertyChanged
    {
        private readonly KundController _kundController;

        public ObservableCollection<Kund> Kunder { get; set; }
        public ICommand TillbakaCommand { get; set; }

        public KundregisterViewModel()
        {
            _kundController = new KundController();
            Kunder = new ObservableCollection<Kund>(_kundController.HämtaAllaKunder());

            SparaKundCommand = new RelayCommand(SparaKund);
            RensaFormularCommand = new RelayCommand(RensaFormular);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
        }

        private Kund _valdKund;
        public Kund ValdKund
        {
            get { return _valdKund; }
            set
            {
                _valdKund = value;
                OnPropertyChanged(nameof(ValdKund));
                // Här kan du lägga in logik för att fylla formuläret om man vill redigera en befintlig kund
            }
        }

        // 0 = Privat, 1 = Företag
        private int _valdKundTypIndex = 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int ValdKundTypIndex
        {
            get { return _valdKundTypIndex; }
            set
            {
                _valdKundTypIndex = value;
                OnPropertyChanged(nameof(ValdKundTypIndex));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string NyEpost { get; set; }
        public string NyTelefon { get; set; }

        public string NyttFörnamn { get; set; }
        public string NyttEfternamn { get; set; }
        public string NyAdress { get; set; }

        public string NyPostnummer { get; set; }

        public string NyOrt {  get; set; }

        public int NyRabatt {  get; set; }

        public int NyKredit { get; set; }
        public string NyttFöretagsnamn { get; set; }

        public ICommand SparaKundCommand { get; set; }
        public ICommand RensaFormularCommand { get; set; }

        

        private void SparaKund(object obj)
        {
            if (ValdKundTypIndex == 0)
            {
                var nyPrivatkund = new PrivatKund
                {
                    KundTyp = "Privat",
                    Epost = NyEpost,
                    Telefonnummer = NyTelefon,
                    Adress = NyAdress,
                    Postnummer = NyPostnummer,
                    Ort = NyOrt,
                    Förnamn = NyttFörnamn,
                    Efternamn = NyttEfternamn,
                    Rabatt = NyRabatt,
                    Kredit = NyKredit,
                };
                _kundController.SkapaKund(nyPrivatkund);
            }
            else if (ValdKundTypIndex == 1)
            {
                var nyForetagsKund = new FöretagsKund
                {
                    KundTyp = "Företag",
                    Epost = NyEpost,
                    FöretagsNamn = NyttFöretagsnamn,
                    Telefonnummer = NyTelefon,
                    Adress = NyAdress,
                    Postnummer = NyPostnummer,
                    Ort = NyOrt,                   
                    Rabatt = NyRabatt,
                    Kredit = NyKredit,
                };
                _kundController.SkapaKund(nyForetagsKund);
            }

            UppdateraKundLista();
            RensaFormular(null);
        }

        private void RensaFormular(object obj)
        {
            NyEpost = string.Empty;
            NyTelefon = string.Empty;
            NyttFörnamn = string.Empty;
            NyttEfternamn = string.Empty;
            NyttFöretagsnamn = string.Empty;

            // Notifiera vyn att uppdatera alla inmatningsfält
            OnPropertyChanged(string.Empty);
        }

        private void UppdateraKundLista()
        {
            Kunder.Clear();
            foreach (var kund in _kundController.HämtaAllaKunder())
            {
                Kunder.Add(kund);
            }
        }

        private void TillbakaTillMeny(object obj)
        {
            Meny menyFonster = new Meny();
            menyFonster.Show();

            // Stäng det nuvarande fönstret
            if (obj is Window nuvarandeFonster)
            {
                nuvarandeFonster.Close();
            }
        }
    }
}