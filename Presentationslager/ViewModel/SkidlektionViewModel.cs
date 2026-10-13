using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        private static readonly string[] DatumFormat =
        {
            "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd",
            "d/M/yyyy", "dd.MM.yyyy", "d.M.yyyy"
        };

        private readonly SkidlektionController _controller;

        private List<Entitetslager.Skidlektion> _alla = new();

        private readonly Dictionary<DateTime, List<Entitetslager.Skidlektion>> _datumCache = new();

        public ObservableCollection<Entitetslager.Skidlektion> VisadeLektioner { get; } = new();
        public ObservableCollection<Personal> Instruktörer { get; } = new();

        private Personal? _valdInstruktör;
        public Personal? ValdInstruktör
        {
            get => _valdInstruktör;
            set
            {
                _valdInstruktör = value;
                OnPropertyChanged();
                FiltreraLektioner();
            }
        }

        private string _söktext = string.Empty;
        public string Söktext
        {
            get => _söktext;
            set
            {
                _söktext = value;
                OnPropertyChanged();
                FiltreraLektioner();
            }
        }

        private string _infoText = string.Empty;
        public string InfoText
        {
            get => _infoText;
            private set
            {
                _infoText = value;
                OnPropertyChanged();
            }
        }

        public ICommand RensaFilterCommand { get; }
        public ICommand StängCommand { get; }

        public SkidlektionViewModel(SkidlektionController controller)
        {
            _controller = controller;

            RensaFilterCommand = new RelayCommand(RensaFilter);
            StängCommand = new RelayCommand(Stäng);

            LaddaData();
        }

        public void LaddaData()
        {
            try
            {
                _alla = _controller.HämtaAktuellaSkidlektioner().ToList();
                _datumCache.Clear();

                Instruktörer.Clear();
                foreach (var instruktör in _controller.HämtaInstruktörer())
                    Instruktörer.Add(instruktör);

                FiltreraLektioner();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte hämta skidlektioner:\n" +
                                (ex.InnerException?.Message ?? ex.Message));
            }
        }

        private void FiltreraLektioner()
        {
            IEnumerable<Entitetslager.Skidlektion> resultat;
            string text = Söktext.Trim();
            bool ärDatumSökning = TryTolkaDatum(text, out DateTime sökDatum);

            if (ärDatumSökning)
            {
                resultat = HämtaLektionerFörDatum(sökDatum);
            }
            else
            {
                resultat = _alla;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    string sök = text.ToLower();

                    resultat = resultat.Where(l =>
                        DatumTexter(l.Datum).Any(d => d.Contains(sök)) ||
                        (l.artikelTyp?.TypNamn?.ToLower().Contains(sök) ?? false) ||
                        (l.Status?.ToLower().Contains(sök) ?? false));
                }
            }

            if (ValdInstruktör != null)
            {
                resultat = resultat.Where(l =>
                    l.AnställningsNummer == ValdInstruktör.AnställningsNummer);
            }

            VisadeLektioner.Clear();
            foreach (var lektion in resultat)
                VisadeLektioner.Add(lektion);

            InfoText = ärDatumSökning
                ? $"Visar {VisadeLektioner.Count} lektioner för {sökDatum:yyyy-MM-dd}"
                : $"Visar {VisadeLektioner.Count} lektioner (kommande två veckor)";
        }

        private IEnumerable<Entitetslager.Skidlektion> HämtaLektionerFörDatum(DateTime datum)
        {
            datum = datum.Date;

            if (_alla.Any(l => l.Datum.Date == datum))
                return _alla.Where(l => l.Datum.Date == datum);

            if (_datumCache.TryGetValue(datum, out var cachade))
                return cachade;

            try
            {
                var lektioner = _controller.HämtaSkidlektionerFörDatum(datum).ToList();
                _datumCache[datum] = lektioner;
                return lektioner;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kunde inte hämta skidlektioner:\n" +
                                (ex.InnerException?.Message ?? ex.Message));
                return Enumerable.Empty<Entitetslager.Skidlektion>();
            }
        }

        private static bool TryTolkaDatum(string text, out DateTime datum)
        {
            return DateTime.TryParseExact(
                text, DatumFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out datum);
        }

        private static IEnumerable<string> DatumTexter(DateTime datum)
        {
            var sv = new CultureInfo("sv-SE");

            yield return datum.ToString("yyyy-MM-dd");
            yield return datum.ToString("dd/MM/yyyy");
            yield return datum.ToString("d/M");
            yield return datum.ToString("dddd", sv).ToLower();
            yield return datum.ToString("MMMM", sv).ToLower();
        }

        private void RensaFilter(object? obj)
        {
            _valdInstruktör = null;
            _söktext = string.Empty;

            OnPropertyChanged(nameof(ValdInstruktör));
            OnPropertyChanged(nameof(Söktext));

            FiltreraLektioner();
        }

        private void Stäng(object? obj)
        {
            if (obj is Window fönster)
                fönster.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}