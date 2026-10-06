using System.Collections.ObjectModel;
using System.Windows.Input;
using Entitetslager;
using Servicelager;
using Presentationslager.Command;
using System.ComponentModel;
using System.Windows;

namespace Presentationslager.ViewModel
{
    public class KundregisterViewModel : INotifyPropertyChanged
    {
        private readonly KundController _kundController;


        public ObservableCollection<Kund> Kunder { get; set; }
        public ICommand TillbakaCommand { get; set; }

        public ICommand UppdateraKundCommand { get; set; }

        public ICommand TaBortKundCommand { get; set; }

        public KundregisterViewModel()
        {
            _kundController = new KundController();
            Kunder = new ObservableCollection<Kund>(_kundController.HämtaAllaKunder());

            SparaKundCommand = new RelayCommand(SparaKund);
            RensaFormularCommand = new RelayCommand(RensaFormular);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            UppdateraKundCommand = new RelayCommand(UppdateraKund);
            TaBortKundCommand = new RelayCommand(TaBortKund);

            NyKredit = 12000;
        }

        private Kund _valdKund;
        public Kund ValdKund
        {
            get { return _valdKund; }
            set
            {
                _valdKund = value;
                OnPropertyChanged(nameof(ValdKund));
                if (ValdKund != null)
                {
                    FyllFormularForUppdatering();
                }
            }
        }


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

        public string NyOrt { get; set; }

        public decimal NyRabatt { get; set; }

        public decimal NyKredit { get; set; }
        public string NyttFöretagsnamn { get; set; }

        public ICommand SparaKundCommand { get; set; }
        public ICommand RensaFormularCommand { get; set; }



        private void SparaKund(object obj)
        {

            if (!ValideraInmatning()) return;

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
                    Kredit = 12000,
                    SenastUppdaterad = DateTime.Now,
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
                    Kredit = 0,
                    SenastUppdaterad = DateTime.Now,

                };
                _kundController.SkapaKund(nyForetagsKund);
            }

            UppdateraKundLista();
            RensaFormular(null);
            MessageBox.Show("Kunden har sparats i registret!", "Sparning genomförd", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RensaFormular(object obj)
        {
        
            NyEpost = string.Empty;
            NyTelefon = string.Empty;
            NyAdress = string.Empty;      
            NyPostnummer = string.Empty; 
            NyOrt = string.Empty;         
            NyRabatt = 0;                 

            NyttFörnamn = string.Empty;
            NyttEfternamn = string.Empty;
            NyttFöretagsnamn = string.Empty;

            OnPropertyChanged(string.Empty);

            if (ValdKundTypIndex == 0)
            {
                NyKredit = 12000;
            }
            else
            {
                NyKredit = 0;
            }
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

        private void FyllFormularForUppdatering()
        {
            NyEpost = ValdKund.Epost;
            NyTelefon = ValdKund.Telefonnummer;
            NyAdress = ValdKund.Adress;
            NyPostnummer = ValdKund.Postnummer;
            NyOrt = ValdKund.Ort;

            if (ValdKund is PrivatKund privat)
            {
                ValdKundTypIndex = 0;
                NyttFörnamn = privat.Förnamn;
                NyttEfternamn = privat.Efternamn;
                NyRabatt = privat.Rabatt;
                NyKredit = privat.Kredit;
            }
            else if (ValdKund is FöretagsKund foretag)
            {
                ValdKundTypIndex = 1;
                NyttFöretagsnamn = foretag.FöretagsNamn;
                NyRabatt = foretag.Rabatt;
                NyKredit = foretag.Kredit;
            }
            OnPropertyChanged(string.Empty);

        }


        private void UppdateraKund(object obj)
        {
            if (ValdKund == null) return;

            if (!ValideraInmatning()) return;

            ValdKund.Epost = NyEpost;
            ValdKund.Telefonnummer = NyTelefon;
            ValdKund.Adress = NyAdress;
            ValdKund.Postnummer = NyPostnummer;
            ValdKund.Ort = NyOrt;

            if (ValdKund is PrivatKund privat)
            {
                privat.Förnamn = NyttFörnamn;
                privat.Efternamn = NyttEfternamn;
                privat.Rabatt = NyRabatt;
                privat.Kredit = NyKredit;
            }
            else if (ValdKund is FöretagsKund foretag)
            {
                foretag.FöretagsNamn = NyttFöretagsnamn;
                foretag.Rabatt = NyRabatt;
                foretag.Kredit = NyKredit;
            }
            _kundController.UppdateraKund(ValdKund);
            UppdateraKundLista();
            RensaFormular(null);

            MessageBox.Show("Kunden har uppdaterats i registret!", "Uppdatering genomförd", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValideraInmatning()
        {
            // 1. Kontrollera att gemensamma fält inte är tomma
            if (string.IsNullOrWhiteSpace(NyEpost) ||
                string.IsNullOrWhiteSpace(NyTelefon) ||
                string.IsNullOrWhiteSpace(NyAdress) ||
                string.IsNullOrWhiteSpace(NyPostnummer) ||
                string.IsNullOrWhiteSpace(NyOrt))
            {
                MessageBox.Show("Alla gemensamma fält (E-post, Telefon, Adress, Postnummer, Ort) måste vara ifyllda.",
                                "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ValdKundTypIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(NyttFörnamn) || string.IsNullOrWhiteSpace(NyttEfternamn))
                {
                    MessageBox.Show("Förnamn och efternamn måste anges för privatkunder.",
                                    "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else if (ValdKundTypIndex == 1) 
            {
                if (string.IsNullOrWhiteSpace(NyttFöretagsnamn))
                {
                    MessageBox.Show("Företagsnamn måste anges för företagskunder.",
                                    "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            if (!NyTelefon.All(char.IsDigit))
            {
                MessageBox.Show("Telefonnumret får endast innehålla siffror.",
                                "Felaktigt format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void TaBortKund(object obj)
        {
            if (ValdKund == null)
            {
                MessageBox.Show("Du måste välja en kund i listan först.",
                                "Ingen kund vald",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult svar = MessageBox.Show(
                $"Är du säker på att du vill ta bort {ValdKund.Epost} permanent?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

       
            if (svar == MessageBoxResult.Yes)
            {
                _kundController.TaBortKund(ValdKund); 
                UppdateraKundLista();                 
                RensaFormular(null);                 

                MessageBox.Show("Kunden har tagits bort från registret.",
                                "Kund borttagen",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
    }
}