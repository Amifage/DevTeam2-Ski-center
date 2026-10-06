using Entitetslager;
using Servicelager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Presentationslager.ViewModel
{
    public class BokningViewModel : INotifyPropertyChanged
    {
        private Kund? _valdKund;
        private DateTime? _startDatum;
        private DateTime? _slutDatum;
        private int? _antalPersonerTotalt;

        private string? _valdBoendeTyp;
        private Logi? _valtLogi;
        private int? _antalPersonerValtLogi;

        private decimal? _aktuelltLogiPris;


        // -------------------------
        // EVENTS TILL FÖNSTRET
        // -------------------------

        public event Action<string>? VisaMeddelande;

        public event Action<string, DateTime, List<LogiRad>>? BokningSparad;

        public event Action? StängFönster;


        // -------------------------
        // LISTOR
        // -------------------------

        public ObservableCollection<Kund> Kunder { get; }

        public ObservableCollection<int> AntalPersonerAlternativ { get; }

        public ObservableCollection<string> BoendeTyper { get; }

        public ObservableCollection<Logi> TillgängligaLogi { get; }

        public ObservableCollection<int> AntalPersonerValtLogiAlternativ { get; }

        public ObservableCollection<LogiRad> LogiRader { get; }


        // -------------------------
        // VALD KUND
        // -------------------------

        public Kund? ValdKund
        {
            get => _valdKund;
            set
            {
                if (_valdKund == value)
                    return;

                _valdKund = value;
                OnPropertyChanged();
            }
        }


        // -------------------------
        // DATUM
        // -------------------------

        public DateTime? StartDatum
        {
            get => _startDatum;
            set
            {
                if (_startDatum == value)
                    return;

                _startDatum = value;

                OnPropertyChanged();

                UppdateraTillgängligaLogi();
                UppdateraPris();
            }
        }


        public DateTime? SlutDatum
        {
            get => _slutDatum;
            set
            {
                if (_slutDatum == value)
                    return;

                _slutDatum = value;

                OnPropertyChanged();

                UppdateraTillgängligaLogi();
                UppdateraPris();
            }
        }


        // -------------------------
        // ANTAL PERSONER TOTALT
        // -------------------------

        public int? AntalPersonerTotalt
        {
            get => _antalPersonerTotalt;
            set
            {
                if (_antalPersonerTotalt == value)
                    return;

                _antalPersonerTotalt = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(BoendePlaceringText));

                UppdateraAntalPersonerFörValtLogi();
            }
        }


        // -------------------------
        // BOENDETYP
        // -------------------------

        public string? ValdBoendeTyp
        {
            get => _valdBoendeTyp;
            set
            {
                if (_valdBoendeTyp == value)
                    return;

                _valdBoendeTyp = value;

                OnPropertyChanged();

                UppdateraTillgängligaLogi();
            }
        }


        // -------------------------
        // VALT LOGI
        // -------------------------

        public Logi? ValtLogi
        {
            get => _valtLogi;
            set
            {
                if (_valtLogi == value)
                    return;

                _valtLogi = value;

                OnPropertyChanged();

                AntalPersonerValtLogi = null;

                UppdateraAntalPersonerFörValtLogi();
                UppdateraPris();
            }
        }


        public int? AntalPersonerValtLogi
        {
            get => _antalPersonerValtLogi;
            set
            {
                if (_antalPersonerValtLogi == value)
                    return;

                _antalPersonerValtLogi = value;

                OnPropertyChanged();
            }
        }


        // -------------------------
        // VISNINGSTEXTER
        // -------------------------

        public string LogiPrisText =>
            _aktuelltLogiPris.HasValue
                ? $"{_aktuelltLogiPris.Value:N0} kr"
                : "0 kr";


        public string BoendePlaceringText
        {
            get
            {
                int totalt = AntalPersonerTotalt ?? 0;

                int placerade =
                    LogiRader.Sum(x => x.AntalPersoner);

                return $"{placerade} av {totalt} personer placerade";
            }
        }


        public string TotalBeloppText
        {
            get
            {
                decimal total =
                    LogiRader.Sum(x => x.LogiBelopp);

                return $"{total:N0} kr";
            }
        }


        // -------------------------
        // COMMANDS
        // -------------------------

        public ICommand LäggTillLogiCommand { get; }

        public ICommand SkapaBokningCommand { get; }

        public ICommand AvbrytCommand { get; }


        // -------------------------
        // CONSTRUCTOR
        // -------------------------

        public BokningViewModel()
        {
            KundController kundController =
                new KundController();

            Kunder =
                new ObservableCollection<Kund>(
                    kundController.HämtaAllaKunder());


            AntalPersonerAlternativ =
                new ObservableCollection<int>(
                    Enumerable.Range(1, 50));


            BoendeTyper =
                new ObservableCollection<string>
                {
                    "Lägenhet",
                    "Camping"
                };


            TillgängligaLogi =
                new ObservableCollection<Logi>();


            AntalPersonerValtLogiAlternativ =
                new ObservableCollection<int>();


            LogiRader =
                new ObservableCollection<LogiRad>();


            LäggTillLogiCommand =
                new RelayCommand(_ => LäggTillLogi());


            SkapaBokningCommand =
                new RelayCommand(_ => SkapaBokning());


            AvbrytCommand =
                new RelayCommand(_ =>
                    StängFönster?.Invoke());
        }


        // =====================================================
        // BOENDE
        // =====================================================

        private void UppdateraTillgängligaLogi()
        {
            TillgängligaLogi.Clear();

            ValtLogi = null;

            if (StartDatum == null ||
                SlutDatum == null ||
                string.IsNullOrWhiteSpace(ValdBoendeTyp))
            {
                return;
            }


            if (SlutDatum < StartDatum)
                return;


            LogiController logiController =
                new LogiController();


            var allaLogi =
                logiController.HämtaAllaLogi();


            var upptagnaLogi =
                logiController.HämtaUpptagnaLogi(
                    StartDatum.Value,
                    SlutDatum.Value);


            IEnumerable<Logi> filtreradeLogi;


            if (ValdBoendeTyp == "Lägenhet")
            {
                filtreradeLogi =
                    allaLogi
                        .Where(l =>
                            l.ArtikelTypNummer == 1 ||
                            l.ArtikelTypNummer == 2)

                        .Where(l =>
                            !LogiRader.Any(r =>
                                r.LogiNummer ==
                                l.LogiNummer))

                        .Where(l =>
                            !upptagnaLogi.Contains(
                                l.LogiNummer))

                        .OrderBy(l =>
                            AvståndTillValdaLogi(l))

                        .ThenBy(l =>
                            HämtaLogiNummer(l));
            }
            else
            {
                filtreradeLogi =
                    allaLogi
                        .Where(l =>
                            l.ArtikelTypNummer == 3)

                        .Where(l =>
                            !LogiRader.Any(r =>
                                r.LogiNummer ==
                                l.LogiNummer))

                        .Where(l =>
                            !upptagnaLogi.Contains(
                                l.LogiNummer))

                        .OrderBy(l =>
                            HämtaLogiNummer(l));
            }


            foreach (Logi logi in filtreradeLogi)
            {
                TillgängligaLogi.Add(logi);
            }
        }


        private void UppdateraAntalPersonerFörValtLogi()
        {
            AntalPersonerValtLogiAlternativ.Clear();

            AntalPersonerValtLogi = null;


            if (ValtLogi == null ||
                AntalPersonerTotalt == null)
            {
                return;
            }


            int redanPlacerade =
                LogiRader.Sum(x =>
                    x.AntalPersoner);


            int återstående =
                AntalPersonerTotalt.Value -
                redanPlacerade;


            if (återstående <= 0)
                return;


            int maxAntal;


            if (ValtLogi.LogiKapacitet.HasValue)
            {
                maxAntal =
                    Math.Min(
                        ValtLogi.LogiKapacitet.Value,
                        återstående);
            }
            else
            {
                // Camping saknar fast kapacitet.
                maxAntal = återstående;
            }


            for (int i = 1; i <= maxAntal; i++)
            {
                AntalPersonerValtLogiAlternativ.Add(i);
            }
        }


        private void UppdateraPris()
        {
            _aktuelltLogiPris = null;

            OnPropertyChanged(
                nameof(LogiPrisText));


            if (ValtLogi == null ||
                StartDatum == null ||
                SlutDatum == null ||
                ValtLogi.ArtikelTypNummer == null)
            {
                return;
            }


            PrisController prisController =
                new PrisController();


            Pris? pris =
                prisController.HämtaAktuelltPris(
                    ValtLogi.ArtikelTypNummer.Value,
                    StartDatum.Value,
                    SlutDatum.Value);


            if (pris != null)
            {
                _aktuelltLogiPris =
                    pris.PrisBelopp;
            }


            OnPropertyChanged(
                nameof(LogiPrisText));
        }


        private void LäggTillLogi()
        {
            if (AntalPersonerTotalt == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja totalt antal personer.");

                return;
            }


            if (ValtLogi == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja ett boende.");

                return;
            }


            if (AntalPersonerValtLogi == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja antal personer för boendet.");

                return;
            }


            if (!ValideraDatum())
                return;


            int redanPlacerade =
                LogiRader.Sum(x =>
                    x.AntalPersoner);


            if (redanPlacerade +
                AntalPersonerValtLogi.Value >
                AntalPersonerTotalt.Value)
            {
                VisaMeddelande?.Invoke(
                    "Du kan inte placera fler personer än det totala antalet i bokningen.");

                return;
            }


            UppdateraPris();


            if (!_aktuelltLogiPris.HasValue)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för det valda boendet.");

                return;
            }


            LogiRad logiRad =
                new LogiRad
                {
                    StartDatum =
                        StartDatum!.Value,

                    SlutDatum =
                        SlutDatum!.Value,

                    LogiBelopp =
                        _aktuelltLogiPris.Value,

                    AntalPersoner =
                        AntalPersonerValtLogi.Value,

                    LogiNummer =
                        ValtLogi.LogiNummer,

                    LogiDisplayText =
                        ValtLogi.DisplayText,

                    SenastUppdaterad =
                        DateTime.Now
                };


            LogiRader.Add(logiRad);


            OnPropertyChanged(
                nameof(BoendePlaceringText));

            OnPropertyChanged(
                nameof(TotalBeloppText));


            ValtLogi = null;

            _aktuelltLogiPris = null;

            OnPropertyChanged(
                nameof(LogiPrisText));


            UppdateraTillgängligaLogi();
        }


        // =====================================================
        // SKAPA BOKNING
        // =====================================================

        private void SkapaBokning()
        {
            if (ValdKund == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja en kund.");

                return;
            }


            if (!ValideraDatum())
                return;


            Entitetslager.Bokning nyBokning =
                new Entitetslager.Bokning
                {
                    KundNummer =
                        ValdKund.KundNummer,

                    BokningsDatum =
                        DateTime.Now,

                    Status =
                        "Aktiv",

                    LogiRader =
                        LogiRader.ToList()
                };


            BokningController bokningController =
                new BokningController();


            bokningController.SkapaBokning(
                nyBokning);


            string kundText =
                ValdKund switch
                {
                    PrivatKund privatKund =>
                        privatKund.DisplayText,

                    FöretagsKund företagsKund =>
                        företagsKund.DisplayText,

                    _ =>
                        ValdKund.KundNummer.ToString()
                };


            BokningSparad?.Invoke(
                kundText,
                DateTime.Now,
                LogiRader.ToList());
        }


        private bool ValideraDatum()
        {
            if (StartDatum == null ||
                SlutDatum == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja både startdatum och slutdatum.");

                return false;
            }


            if (SlutDatum < StartDatum)
            {
                VisaMeddelande?.Invoke(
                    "Slutdatum kan inte vara före startdatum.");

                return false;
            }


            return true;
        }


        // =====================================================
        // ÅTERSTÄLL
        // =====================================================

        public void ÅterställFormulär()
        {
            LogiRader.Clear();

            ValdKund = null;

            StartDatum = null;
            SlutDatum = null;

            AntalPersonerTotalt = null;

            ValdBoendeTyp = null;

            ValtLogi = null;

            AntalPersonerValtLogi = null;

            TillgängligaLogi.Clear();

            AntalPersonerValtLogiAlternativ.Clear();

            _aktuelltLogiPris = null;


            OnPropertyChanged(
                nameof(LogiPrisText));

            OnPropertyChanged(
                nameof(BoendePlaceringText));

            OnPropertyChanged(
                nameof(TotalBeloppText));
        }


        // =====================================================
        // HJÄLPMETODER
        // =====================================================

        private int HämtaLogiNummer(Logi logi)
        {
            if (string.IsNullOrWhiteSpace(
                logi.LogiNummer))
            {
                return int.MaxValue;
            }


            string siffror =
                new string(
                    logi.LogiNummer
                        .Where(char.IsDigit)
                        .ToArray());


            return int.TryParse(
                siffror,
                out int nummer)

                ? nummer
                : int.MaxValue;
        }


        private int AvståndTillValdaLogi(
            Logi logi)
        {
            if (!LogiRader.Any())
                return 0;


            int aktuelltNummer =
                HämtaLogiNummer(logi);


            return LogiRader
                .Where(r =>
                    !string.IsNullOrWhiteSpace(
                        r.LogiNummer))

                .Select(r =>
                {
                    string siffror =
                        new string(
                            r.LogiNummer!
                                .Where(char.IsDigit)
                                .ToArray());


                    return int.TryParse(
                        siffror,
                        out int nummer)

                        ? Math.Abs(
                            aktuelltNummer -
                            nummer)

                        : int.MaxValue;
                })

                .DefaultIfEmpty(0)
                .Min();
        }


        // =====================================================
        // PROPERTY CHANGED
        // =====================================================

        public event PropertyChangedEventHandler?
            PropertyChanged;


        protected void OnPropertyChanged(
            [CallerMemberName]
            string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(
                    propertyName));
        }


        // =====================================================
        // RELAY COMMAND
        // =====================================================

        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?>
                _execute;

            private readonly Predicate<object?>?
                _canExecute;


            public RelayCommand(
                Action<object?> execute,
                Predicate<object?>? canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }


            public bool CanExecute(
                object? parameter)
            {
                return _canExecute == null ||
                       _canExecute(parameter);
            }


            public void Execute(
                object? parameter)
            {
                _execute(parameter);
            }


            public event EventHandler?
                CanExecuteChanged;


            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(
                    this,
                    EventArgs.Empty);
            }
        }
    }
}