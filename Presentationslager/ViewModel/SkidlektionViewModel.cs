using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Entitetslager;
using Presentationslager.Command;
using Servicelager;

namespace Presentationslager.ViewModel
{
    public class SkidlektionViewModel : INotifyPropertyChanged
    {
        private readonly PersonalController _personalController;

        private ObservableCollection<Entitetslager.Skidlektion> _skidlektionsLista;
        public ObservableCollection<Entitetslager.Skidlektion> SkidlektionsLista
        {
            get => _skidlektionsLista;
            set { _skidlektionsLista = value; OnPropertyChanged(); }
        }

        private Entitetslager.Skidlektion? _valdSkidlektion;
        public Entitetslager.Skidlektion? ValdSkidlektion
        {
            get => _valdSkidlektion;
            set
            {
                _valdSkidlektion = value;
                OnPropertyChanged();

                NyInstruktor = value == null
                    ? null
                    : AllaAnstallda.FirstOrDefault(p => p.AnställningsNummer == value.AnställningsNummer);
            }
        }

        private string _sokDatum = string.Empty;
        public string SokDatum
        {
            get => _sokDatum;
            set { _sokDatum = value; OnPropertyChanged(); }
        }

        private string _sokStatus = string.Empty;
        public string SokStatus
        {
            get => _sokStatus;
            set { _sokStatus = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> _artikelTyper;
        public ObservableCollection<string> ArtikelTyper
        {
            get => _artikelTyper;
            set { _artikelTyper = value; OnPropertyChanged(); }
        }

        private string? _valdArtikelTyp;
        public string? ValdArtikelTyp
        {
            get => _valdArtikelTyp;
            set { _valdArtikelTyp = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Personal> _instruktorer;
        public ObservableCollection<Personal> Instruktorer
        {
            get => _instruktorer;
            set { _instruktorer = value; OnPropertyChanged(); }
        }

        private Personal? _valdInstruktor;
        public Personal? ValdInstruktor
        {
            get => _valdInstruktor;
            set { _valdInstruktor = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Personal> _allaAnstallda;
        public ObservableCollection<Personal> AllaAnstallda
        {
            get => _allaAnstallda;
            set { _allaAnstallda = value; OnPropertyChanged(); }
        }

        private Personal? _nyInstruktor;
        public Personal? NyInstruktor
        {
            get => _nyInstruktor;
            set { _nyInstruktor = value; OnPropertyChanged(); }
        }

        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraInstruktorCommand { get; }
        public ICommand TillbakaCommand { get; set; }

        public SkidlektionViewModel()
        {
            _personalController = new PersonalController();

            SkidlektionsLista = new ObservableCollection<Entitetslager.Skidlektion>();
            ArtikelTyper = new ObservableCollection<string>();
            Instruktorer = new ObservableCollection<Personal>();
            AllaAnstallda = new ObservableCollection<Personal>();

            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);
            UppdateraInstruktorCommand = new RelayCommand(UppdateraInstruktor, CanUppdateraInstruktor);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            LaddaData();
        }

        private void LaddaData()
        {
            var lektioner = _personalController.HamtaAktuellaSkidlektioner();
            SkidlektionsLista.Clear();
            if (lektioner != null)
            {
                foreach (Entitetslager.Skidlektion item in lektioner)
                {
                    SkidlektionsLista.Add(item);
                }
            }

            var typer = _personalController.HamtaAllaArtikelTyper();
            ArtikelTyper.Clear();
            if (typer != null)
            {
                var lektionsTyper = typer
                    .Where(t => t != null && !string.IsNullOrWhiteSpace(t.TypNamn))
                    .Select(t => t.TypNamn)
                    .Where(namn => namn.Contains("lektion", StringComparison.OrdinalIgnoreCase));

                foreach (var typ in lektionsTyper)
                {
                    ArtikelTyper.Add(typ);
                }
            }

            var instruktorer = _personalController.HamtaInstruktorer();
            Instruktorer.Clear();
            if (instruktorer != null)
            {
                foreach (Personal item in instruktorer)
                {
                    Instruktorer.Add(item);
                }
            }

            var anstallda = _personalController.HamtaAllaAnstallda();
            AllaAnstallda.Clear();
            if (anstallda != null)
            {
                foreach (Personal item in anstallda)
                {
                    AllaAnstallda.Add(item);
                }
            }
        }

        private void UtforSokning(object obj)
        {
            var resultat = _personalController.SokSkidlektioner(
                SokDatum,
                SokStatus,
                ValdArtikelTyp,
                ValdInstruktor?.AnställningsNummer
            );

            SkidlektionsLista.Clear();
            if (resultat != null)
            {
                foreach (Entitetslager.Skidlektion item in resultat)
                {
                    SkidlektionsLista.Add(item);
                }
            }
        }

        private void RensaFalt(object obj)
        {
            SokDatum = string.Empty;
            SokStatus = string.Empty;
            ValdArtikelTyp = null;
            ValdInstruktor = null;

            LaddaData();
        }

        private bool CanUppdateraInstruktor(object obj)
        {
            return ValdSkidlektion != null &&
                   NyInstruktor != null &&
                   NyInstruktor.AnställningsNummer != ValdSkidlektion.AnställningsNummer;
        }

        private void UppdateraInstruktor(object obj)
        {
            if (ValdSkidlektion == null || NyInstruktor == null) return;

            var vald = ValdSkidlektion;
            var nyInstruktor = NyInstruktor;

            if (_personalController.HarOverlappandeLektion(vald.SkidlektionNummer, nyInstruktor.AnställningsNummer))
            {
                MessageBox.Show("Den valda anställda har redan en annan lektion som överlappar i tid.",
                                "Kan inte byta instruktör", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool lyckades = _personalController.UppdateraInstruktor(vald.SkidlektionNummer, nyInstruktor.AnställningsNummer);

            if (lyckades)
            {
                int index = SkidlektionsLista.IndexOf(vald);
                if (index >= 0)
                {
                    SkidlektionsLista[index] = vald;
                }

                ValdSkidlektion = vald;

                MessageBox.Show($"Instruktör för lektion {vald.SkidlektionNummer} uppdaterades till: {nyInstruktor.Förnamn} {nyInstruktor.Efternamn}",
                                "Instruktör uppdaterad", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Det gick inte att uppdatera instruktören för den valda lektionen.",
                                "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}