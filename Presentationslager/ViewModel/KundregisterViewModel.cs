using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Entitetslager;
using Servicelager;
using Presentationslager.Command;

namespace Presentationslager.ViewModel
{
    public class KundregisterViewModel : INotifyPropertyChanged
    {
        private readonly KundController _kundController;

        public ObservableCollection<Kund> Kunder { get; set; }

        public ICollectionView FiltreradeKunder { get; set; }

        public ICommand TillbakaCommand { get; set; }
        public ICommand UppdateraKundCommand { get; set; }
        public ICommand TaBortKundCommand { get; set; }
        public ICommand SparaKundCommand { get; set; }
        public ICommand RensaFormularCommand { get; set; }



        public KundregisterViewModel()
        {
            _kundController = new KundController();
            Kunder = new ObservableCollection<Kund>(_kundController.HamtaAllaKunder());

            _kundController.UppdateraPrivatkundersRabatt();
            FiltreradeKunder = CollectionViewSource.GetDefaultView(Kunder);
            FiltreradeKunder.Filter = FiltreraKundLista;

            SparaKundCommand = new RelayCommand(SparaKund);
            RensaFormularCommand = new RelayCommand(RensaFormular);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            UppdateraKundCommand = new RelayCommand(UppdateraKund, CanUppdateraKund);
            TaBortKundCommand = new RelayCommand(TaBortKund, CanUppdateraKund);

            NyKredit = 12000;
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

        public bool KanAndraKreditOchRabatt
        {
            get { return InloggningController.KanSattaKreditOchRabatt; }
        }

        #region Metoder
        private string _sokText;
        public string SokText
        {
            get { return _sokText; }
            set
            {
                _sokText = value;
                OnPropertyChanged(nameof(SokText));
                FiltreradeKunder?.Refresh();
            }
        }

        private int _valdFilterTypIndex = 0; 
        public int ValdFilterTypIndex
        {
            get { return _valdFilterTypIndex; }
            set
            {
                _valdFilterTypIndex = value;
                OnPropertyChanged(nameof(ValdFilterTypIndex));
                FiltreradeKunder?.Refresh();
            }
        }
      
        private bool FiltreraKundLista(object obj)
        {
            if (obj is Kund kund)
            {
                
                bool typMatchar = true;
                if (ValdFilterTypIndex == 1)
                    typMatchar = kund is PrivatKund;
                else if (ValdFilterTypIndex == 2)
                    typMatchar = kund is FöretagsKund;

                
                bool sokMatchar = true;
                if (!string.IsNullOrWhiteSpace(SokText))
                {
                    string sokning = SokText.ToLower();
                    sokMatchar = (kund.VisningsNamn?.ToLower().Contains(sokning) == true) ||
                                 (kund.Ort?.ToLower().Contains(sokning) == true) ||
                                 (kund.Epost?.ToLower().Contains(sokning) == true) ||
                                 (kund.KundNummer.ToString().Contains(sokning));
                }

                return typMatchar && sokMatchar;
            }
            return false;
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

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _valdKundTypIndex = 0;
        public int ValdKundTypIndex
        {
            get { return _valdKundTypIndex; }
            set
            {
                _valdKundTypIndex = value;
                OnPropertyChanged(nameof(ValdKundTypIndex));
            }
        }      

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
                    Rabatt = KanAndraKreditOchRabatt ? NyRabatt : 0,
                    Kredit = 0,
                    SenastUppdaterad = DateTime.Now,
                };
                _kundController.SkapaKund(nyForetagsKund);
            }

            UppdateraKundLista();
            RensaFormular(null);
            MessageBox.Show("En ny kund har välkomnats till SKi-Center familjen!", "Sparning genomförd", MessageBoxButton.OK, MessageBoxImage.Information);
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
            UppdateraKundLista();

        }

        private void UppdateraKundLista()
        {
            Kunder.Clear();
            foreach (var kund in _kundController.HamtaAllaKunder())
            {
                Kunder.Add(kund);
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

        private bool CanUppdateraKund(object obj)
        {
            return ValdKund != null;
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
            }
            else if (ValdKund is FöretagsKund foretag)
            {
                foretag.FöretagsNamn = NyttFöretagsnamn;

                if (KanAndraKreditOchRabatt)
                {
                    foretag.Rabatt = NyRabatt;
                    foretag.Kredit = NyKredit;
                }
            }

            _kundController.UppdateraKund(ValdKund);
            UppdateraKundLista();
            RensaFormular(null);

            MessageBox.Show("Uppdaterat och klart! Nu stämmer allt till punkt och pricka.", "Uppdatering genomförd", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValideraInmatning()
        {
            if (string.IsNullOrWhiteSpace(NyEpost) ||
                string.IsNullOrWhiteSpace(NyTelefon) ||
                string.IsNullOrWhiteSpace(NyAdress) ||
                string.IsNullOrWhiteSpace(NyPostnummer) ||
                string.IsNullOrWhiteSpace(NyOrt))
            {
                MessageBox.Show("Vi saknar lite information, alla fält måste vara ifyllda! (E-post, Telefon, Adress, Postnummer, Ort)",
                                "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (ValdKundTypIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(NyttFörnamn) || string.IsNullOrWhiteSpace(NyttEfternamn))
                {
                    MessageBox.Show("Vi behöver veta exakt vem kunden är! Fyll i både för-och efternamn för att kunna spara.",
                                    "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else if (ValdKundTypIndex == 1)
            {
                if (string.IsNullOrWhiteSpace(NyttFöretagsnamn))
                {
                    MessageBox.Show("Hoppsan, namnet på företaget glömdes visst! Skriv in det så vi kan spara dem!",
                                    "Information saknas", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }        

            return true;
        }

        private void TaBortKund(object obj)
        {       

            MessageBoxResult svar = MessageBox.Show(
                $"Är du säker på att du vill ta bort {ValdKund.VisningsNamn} permanent?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (svar == MessageBoxResult.Yes)
            {
                _kundController.TaBortKund(ValdKund);
                UppdateraKundLista();
                RensaFormular(null);

                MessageBox.Show("Kunden har lämnat Ski-Center familjen :(",
                                "Kund borttagen",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        } 
        #endregion  
    }
}