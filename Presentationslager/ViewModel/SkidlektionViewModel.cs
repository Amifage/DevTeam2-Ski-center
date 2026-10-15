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
        private readonly SkidlektionController _skidlektionController;

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

                NyInstruktör = value == null
                    ? null
                    : AllaAnställda.FirstOrDefault(p => p.AnställningsNummer == value.AnställningsNummer);
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

        private ObservableCollection<Personal> _instruktörer;
        public ObservableCollection<Personal> Instruktörer
        {
            get => _instruktörer;
            set { _instruktörer = value; OnPropertyChanged(); }
        }

        private Personal? _valdInstruktör;
        public Personal? ValdInstruktör
        {
            get => _valdInstruktör;
            set { _valdInstruktör = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Personal> _allaAnställda;
        public ObservableCollection<Personal> AllaAnställda
        {
            get => _allaAnställda;
            set { _allaAnställda = value; OnPropertyChanged(); }
        }

        private Personal? _nyInstruktör;
        public Personal? NyInstruktör
        {
            get => _nyInstruktör;
            set { _nyInstruktör = value; OnPropertyChanged(); }
        }

        public ICommand SokCommand { get; }
        public ICommand RensaCommand { get; }
        public ICommand UppdateraInstruktörCommand { get; }
        public ICommand TillbakaCommand { get; set; }

        public SkidlektionViewModel()
        {
            _skidlektionController = new SkidlektionController();

            SkidlektionsLista = new ObservableCollection<Entitetslager.Skidlektion>();
            ArtikelTyper = new ObservableCollection<string>();
            Instruktörer = new ObservableCollection<Personal>();
            AllaAnställda = new ObservableCollection<Personal>();

            SokCommand = new RelayCommand(UtforSokning);
            RensaCommand = new RelayCommand(RensaFalt);
            UppdateraInstruktörCommand = new RelayCommand(UppdateraInstruktör, CanUppdateraInstruktör);
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
            LaddaData();
        }

        private void LaddaData()
        {
            var lektioner = _skidlektionController.HamtaAktuellaSkidlektioner();
            SkidlektionsLista.Clear();
            if (lektioner != null)
            {
                foreach (Entitetslager.Skidlektion item in lektioner)
                {
                    SkidlektionsLista.Add(item);
                }
            }

            var typer = _skidlektionController.HamtaAllaArtikelTyper();
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

            var instruktörer = _skidlektionController.HamtaInstruktorer();
            Instruktörer.Clear();
            if (instruktörer != null)
            {
                foreach (Personal item in instruktörer)
                {
                    Instruktörer.Add(item);
                }
            }

            var anställda = _skidlektionController.HamtaAllaAnstallda();
            AllaAnställda.Clear();
            if (anställda != null)
            {
                foreach (Personal item in anställda)
                {
                    AllaAnställda.Add(item);
                }
            }
        }

        private void UtforSokning(object obj)
        {
            var resultat = _skidlektionController.SokSkidlektioner(
                SokDatum,
                SokStatus,
                ValdArtikelTyp,
                ValdInstruktör?.AnställningsNummer
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
            ValdInstruktör = null;

            LaddaData();
        }

        private bool CanUppdateraInstruktör(object obj)
        {
            return ValdSkidlektion != null &&
                   NyInstruktör != null &&
                   NyInstruktör.AnställningsNummer != ValdSkidlektion.AnställningsNummer;
        }

        private void UppdateraInstruktör(object obj)
        {
            if (ValdSkidlektion == null || NyInstruktör == null) return;

            var vald = ValdSkidlektion;
            var nyInstruktör = NyInstruktör;

            if (_skidlektionController.HarOverlappandeLektion(vald.SkidlektionNummer, nyInstruktör.AnställningsNummer))
            {
                MessageBox.Show("Den valda anställda har redan en annan lektion som överlappar i tid.",
                                "Kan inte byta instruktör", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool lyckades = _skidlektionController.UppdateraInstruktor(vald.SkidlektionNummer, nyInstruktör.AnställningsNummer);

            if (lyckades)
            {
                int index = SkidlektionsLista.IndexOf(vald);
                if (index >= 0)
                {
                    SkidlektionsLista[index] = vald;
                }

                ValdSkidlektion = vald;

                MessageBox.Show($"Instruktör för lektion {vald.SkidlektionNummer} uppdaterades till: {nyInstruktör.Förnamn} {nyInstruktör.Efternamn}",
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