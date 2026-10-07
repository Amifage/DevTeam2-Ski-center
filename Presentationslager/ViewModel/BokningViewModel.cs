using Entitetslager;
using Servicelager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using UtrustningEntitet = Entitetslager.Utrustning;

namespace Presentationslager.ViewModel
{
    public class BokningViewModel : INotifyPropertyChanged
    {
        #region Fält

        private Kund? _valdKund;

        private DateTime? _startDatum;
        private DateTime? _slutDatum;

        private int? _antalPersonerTotalt;

        private string? _valdBoendeTyp;
        private Logi? _valtLogi;
        private ArtikelTyp? _valdLogiArtikelTyp;
        private int? _antalPersonerValtLogi;
        private decimal? _aktuelltLogiPris;

        private string? _valdUtrustningTyp;
        private UtrustningPaket? _valtUtrustningPaket;
        private UtrustningEntitet? _valdEnskildUtrustning;
        private ArtikelTyp? _valdUtrustningArtikelTyp;
        private int? _antalUtrustningsPaket;
        private decimal? _aktuelltUtrustningPris;

        #endregion

        #region Events

        public event Action<string>? VisaMeddelande;
        public event Action<string, DateTime, List<LogiRad>, List<UtrustningRad>>? BokningSparad;
        public event Action? StängFönster;

        #endregion

        #region Collections

        public ObservableCollection<Kund> Kunder { get; }

        public ObservableCollection<int> AntalPersonerAlternativ { get; }

        public ObservableCollection<string> BoendeTyper { get; }
        public ObservableCollection<Logi> TillgängligaLogi { get; }
        public ObservableCollection<ArtikelTyp> TillgängligaLogiArtikelTyper { get; }
        public ObservableCollection<int> AntalPersonerValtLogiAlternativ { get; }
        public ObservableCollection<LogiRad> LogiRader { get; }
        public ObservableCollection<string> TillgängligaLogiTypTexter { get; }

        public ObservableCollection<string> UtrustningTyper { get; }
        public ObservableCollection<UtrustningPaket> TillgängligaUtrustningsPaket { get; }
        public ObservableCollection<UtrustningEntitet> TillgängligaEnskildaUtrustningar { get; }
        public ObservableCollection<ArtikelTyp> TillgängligaUtrustningArtikelTyper { get; }
        public ObservableCollection<int> AntalUtrustningsPaketAlternativ { get; }
        public ObservableCollection<UtrustningRad> UtrustningRader { get; }

        #endregion

        #region Commands

        public ICommand LäggTillLogiCommand { get; }
        public ICommand LäggTillUtrustningCommand { get; }
        public ICommand SkapaBokningCommand { get; }
        public ICommand AvbrytCommand { get; }

        #endregion

        #region Konstruktor

        public BokningViewModel()
        {
            KundController kundController = new KundController();

            Kunder = new ObservableCollection<Kund>(kundController.HämtaAllaKunder());
            AntalPersonerAlternativ = new ObservableCollection<int>(Enumerable.Range(1, 50));

            BoendeTyper = new ObservableCollection<string>
            {
                "Lägenhet",
                "Camping"
            };

            TillgängligaLogi = new ObservableCollection<Logi>();
            TillgängligaLogiArtikelTyper = new ObservableCollection<ArtikelTyp>();
            AntalPersonerValtLogiAlternativ = new ObservableCollection<int>();
            LogiRader = new ObservableCollection<LogiRad>();
            TillgängligaLogiTypTexter = new ObservableCollection<string>();

            UtrustningTyper = new ObservableCollection<string>
            {
                "Utrustningspaket",
                "Enskild utrustning"
            };

            TillgängligaUtrustningsPaket = new ObservableCollection<UtrustningPaket>();
            TillgängligaEnskildaUtrustningar = new ObservableCollection<UtrustningEntitet>();
            TillgängligaUtrustningArtikelTyper = new ObservableCollection<ArtikelTyp>();
            AntalUtrustningsPaketAlternativ = new ObservableCollection<int>();
            UtrustningRader = new ObservableCollection<UtrustningRad>();

            LäggTillLogiCommand = new RelayCommand(_ => LäggTillLogi());
            LäggTillUtrustningCommand = new RelayCommand(_ => LäggTillUtrustning());
            SkapaBokningCommand = new RelayCommand(_ => SkapaBokning());
            AvbrytCommand = new RelayCommand(_ => StängFönster?.Invoke());
        }

        #endregion

        #region Kund

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

        #endregion

        #region Datum

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
                UppdateraLogiPris();
                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();
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
                UppdateraLogiPris();
                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();
            }
        }

        #endregion

        #region Antal personer

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
                UppdateraAntalPaketAlternativ();
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

        #endregion



        #region Logi - val

        public string? ValdBoendeTyp
        {
            get => _valdBoendeTyp;
            set
            {
                if (_valdBoendeTyp == value)
                    return;

                _valdBoendeTyp = value;
                OnPropertyChanged();

                ValdLogiArtikelTyp = null;
                UppdateraTillgängligaLogi();
            }
        }

        public ArtikelTyp? ValdLogiArtikelTyp
        {
            get => _valdLogiArtikelTyp;
            set
            {
                if (_valdLogiArtikelTyp == value)
                    return;

                _valdLogiArtikelTyp = value;
                OnPropertyChanged();

                UppdateraLogiPris();
                UppdateraAntalPersonerFörValtLogi();
            }
        }

        public Logi? ValtLogi
        {
            get => _valtLogi;
            set
            {
                if (_valtLogi == value)
                    return;

                _valtLogi = value;
                OnPropertyChanged();

                ValdLogiArtikelTyp = _valtLogi?.ArtikelTypNummer == null
                    ? null
                    : TillgängligaLogiArtikelTyper.FirstOrDefault(
                        a => a.ArtikelTypNummer == _valtLogi.ArtikelTypNummer);
            }
        }

        #endregion

        #region Logi - visning
        public bool FinnsTillgängligaLogi => TillgängligaLogi.Count > 0;

        public bool VisaIngaTillgängligaLogi =>
                    StartDatum.HasValue &&
                    SlutDatum.HasValue &&
                    !string.IsNullOrWhiteSpace(ValdBoendeTyp) &&
                    !FinnsTillgängligaLogi;

        public string LogiPrisText => _aktuelltLogiPris.HasValue ? $"{_aktuelltLogiPris.Value:N0} kr" : "0 kr";

        public string BoendePlaceringText
        {
            get
            {
                int totalt = AntalPersonerTotalt ?? 0;
                int placerade = LogiRader.Sum(x => x.AntalPersoner);

                return $"{placerade} av {totalt} personer placerade";
            }
        }

        #endregion

        #region Logi - tillgänglighet

        private void UppdateraTillgängligaLogi()
        {
            TillgängligaLogi.Clear();
            TillgängligaLogiArtikelTyper.Clear();
            TillgängligaLogiTypTexter.Clear();

            OnPropertyChanged(nameof(FinnsTillgängligaLogi));
            OnPropertyChanged(nameof(VisaIngaTillgängligaLogi));

            ValtLogi = null;

            if (StartDatum == null ||
                SlutDatum == null ||
                string.IsNullOrWhiteSpace(ValdBoendeTyp))
            {
                return;
            }

            if (SlutDatum < StartDatum)
                return;

            LogiController logiController = new LogiController();
            UtrustningController utrustningController = new UtrustningController();

            var allaLogi = logiController.HämtaAllaLogi();
            var upptagnaLogi = logiController.HämtaUpptagnaLogi(StartDatum.Value, SlutDatum.Value);

            IEnumerable<Logi> ledigaLogi = allaLogi
                .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                .Where(l => !LogiRader.Any(r => r.LogiNummer == l.LogiNummer));

            if (ValdBoendeTyp == "Lägenhet")
            {
                ledigaLogi = ledigaLogi
                    .Where(l => l.ArtikelTypNummer == 1 || l.ArtikelTypNummer == 2);
            }
            else if (ValdBoendeTyp == "Camping")
            {
                ledigaLogi = ledigaLogi
                    .Where(l => l.ArtikelTypNummer == 3);
            }

            List<Logi> ledigaLogiLista = ledigaLogi.ToList();

            var representativaLogi = ledigaLogiLista
                .Where(l => l.ArtikelTypNummer.HasValue)
                .GroupBy(l => l.ArtikelTypNummer)
                .Select(g => g.First())
                .OrderBy(l => l.ArtikelTypNummer)
                .ToList();
           
            var allaArtikelTyper = utrustningController.HamtaAllaArtikelTyper();

            foreach (Logi logi in representativaLogi)
            {
                string typNamn = allaArtikelTyper
                    .FirstOrDefault(a => a.ArtikelTypNummer == logi.ArtikelTypNummer)
                    ?.TypNamn ?? "";

                logi.TypDisplayText =
                    $"{typNamn} | {logi.LogiKapacitet} pers | {logi.Storlek} kvm | {logi.Faciliteter} | {logi.AntalRum} rum";
            }

            foreach (Logi logi in representativaLogi)
                TillgängligaLogi.Add(logi);

            OnPropertyChanged(nameof(FinnsTillgängligaLogi));
            OnPropertyChanged(nameof(VisaIngaTillgängligaLogi));

            var ledigaArtikelTypNummer = ledigaLogiLista
                .Where(l => l.ArtikelTypNummer.HasValue)
                .Select(l => l.ArtikelTypNummer!.Value)
                .Distinct()
                .ToList();

            foreach (ArtikelTyp artikelTyp in allaArtikelTyper)
            {
                if (!ledigaArtikelTypNummer.Contains(artikelTyp.ArtikelTypNummer))
                    continue;

                TillgängligaLogiArtikelTyper.Add(artikelTyp);

                Logi? exempelLogi = ledigaLogiLista
                    .FirstOrDefault(l => l.ArtikelTypNummer == artikelTyp.ArtikelTypNummer);

                if (exempelLogi == null)
                    continue;

                string text = artikelTyp.TypNamn;

                if (exempelLogi.LogiKapacitet.HasValue)
                    text += $" | {exempelLogi.LogiKapacitet.Value} pers";

                if (exempelLogi.Storlek.HasValue)
                    text += $" | {exempelLogi.Storlek.Value} kvm";

                if (!string.IsNullOrWhiteSpace(exempelLogi.Faciliteter))
                    text += $" | {exempelLogi.Faciliteter}";

                if (exempelLogi.AntalRum.HasValue)
                    text += $" | {exempelLogi.AntalRum.Value} rum";

                TillgängligaLogiTypTexter.Add(text);
            }
        }

        private void UppdateraAntalPersonerFörValtLogi()
        {
            AntalPersonerValtLogiAlternativ.Clear();
            AntalPersonerValtLogi = null;

            if (ValdLogiArtikelTyp == null || AntalPersonerTotalt == null)
                return;

            int redanPlacerade = LogiRader.Sum(x => x.AntalPersoner);
            int återstående = AntalPersonerTotalt.Value - redanPlacerade;

            if (återstående <= 0)
                return;

            LogiController logiController = new LogiController();

            Logi? logiAvValdTyp = logiController
                .HämtaAllaLogi()
                .FirstOrDefault(l => l.ArtikelTypNummer == ValdLogiArtikelTyp.ArtikelTypNummer);

            if (logiAvValdTyp == null)
                return;

            int maxAntal = logiAvValdTyp.LogiKapacitet.HasValue
                ? Math.Min(logiAvValdTyp.LogiKapacitet.Value, återstående)
                : återstående;

            for (int i = 1; i <= maxAntal; i++)
                AntalPersonerValtLogiAlternativ.Add(i);
        }

        #endregion

        #region Logi - pris

        private void UppdateraLogiPris()
        {
            _aktuelltLogiPris = null;
            OnPropertyChanged(nameof(LogiPrisText));

            if (ValdLogiArtikelTyp == null || StartDatum == null || SlutDatum == null)
                return;

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaAktuelltPris(
                ValdLogiArtikelTyp.ArtikelTypNummer,
                StartDatum.Value,
                SlutDatum.Value);

            if (pris != null)
                _aktuelltLogiPris = pris.PrisBelopp;

            OnPropertyChanged(nameof(LogiPrisText));
        }

        #endregion

        #region Logi - lägg till

        private void LäggTillLogi()
        {
            if (AntalPersonerTotalt == null)
            {
                VisaMeddelande?.Invoke("Du måste välja totalt antal personer.");
                return;
            }

            if (ValdLogiArtikelTyp == null)
            {
                VisaMeddelande?.Invoke("Du måste välja vilken typ av boende du vill boka.");
                return;
            }

            if (AntalPersonerValtLogi == null)
            {
                VisaMeddelande?.Invoke("Du måste välja antal personer för boendet.");
                return;
            }

            if (!ValideraDatum())
                return;

            int redanPlacerade = LogiRader.Sum(x => x.AntalPersoner);

            if (redanPlacerade + AntalPersonerValtLogi.Value > AntalPersonerTotalt.Value)
            {
                VisaMeddelande?.Invoke("Du kan inte placera fler personer än det totala antalet i bokningen.");

                return;
            }

            LogiController logiController = new LogiController();

            var allaLogi = logiController.HämtaAllaLogi();
            var upptagnaLogi = logiController.HämtaUpptagnaLogi(StartDatum!.Value, SlutDatum!.Value);

            Logi? valtFysisktLogi = allaLogi
                .Where(l => l.ArtikelTypNummer == ValdLogiArtikelTyp.ArtikelTypNummer)
                .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                .Where(l => !LogiRader.Any(r => r.LogiNummer == l.LogiNummer))
                .OrderBy(l => AvståndTillValdaLogi(l))
                .ThenBy(l => HämtaLogiNummer(l))
                .FirstOrDefault();

            if (valtFysisktLogi == null)
            {
                VisaMeddelande?.Invoke("Det finns inget ledigt boende av den valda typen.");
                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaAktuelltPris(
                ValdLogiArtikelTyp.ArtikelTypNummer,
                StartDatum.Value,
                SlutDatum.Value);

            if (pris == null)
            {
                VisaMeddelande?.Invoke("Kunde inte hitta något pris för den valda boendetypen.");
                return;
            }

            LogiRad logiRad = new LogiRad
            {
                StartDatum = StartDatum.Value,
                SlutDatum = SlutDatum.Value,
                LogiBelopp = pris.PrisBelopp,
                AntalPersoner = AntalPersonerValtLogi.Value,
                LogiNummer = valtFysisktLogi.LogiNummer,
                LogiDisplayText = ValtLogi?.TypDisplayText ?? ValdLogiArtikelTyp.TypNamn,
                SenastUppdaterad = DateTime.Now
            };

            LogiRader.Add(logiRad);

            OnPropertyChanged(nameof(BoendePlaceringText));
            OnPropertyChanged(nameof(TotalBeloppText));

            ValdLogiArtikelTyp = null;
            AntalPersonerValtLogi = null;
            _aktuelltLogiPris = null;

            OnPropertyChanged(nameof(LogiPrisText));

            UppdateraTillgängligaLogi();
        }
        #endregion


        #region Hjälpmetoder Logi

        private int HämtaLogiNummer(Logi logi)
        {
            if (string.IsNullOrWhiteSpace(logi.LogiNummer))
                return int.MaxValue;

            string siffror = new string(logi.LogiNummer
                .Where(char.IsDigit)
                .ToArray());

            return int.TryParse(siffror, out int nummer)
                ? nummer
                : int.MaxValue;
        }

        private int AvståndTillValdaLogi(Logi logi)
        {
            if (!LogiRader.Any())
                return 0;

            int aktuelltNummer = HämtaLogiNummer(logi);

            return LogiRader
                .Where(r => !string.IsNullOrWhiteSpace(r.LogiNummer))
                .Select(r =>
                {
                    string siffror = new string(r.LogiNummer!
                        .Where(char.IsDigit)
                        .ToArray());

                    return int.TryParse(siffror, out int nummer)
                        ? Math.Abs(aktuelltNummer - nummer)
                        : int.MaxValue;
                })
                .DefaultIfEmpty(0)
                .Min();
        }

        #endregion




        #region Utrustning - val

        public string? ValdUtrustningTyp
        {
            get => _valdUtrustningTyp;
            set
            {
                if (_valdUtrustningTyp == value)
                    return;

                _valdUtrustningTyp = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(VisarUtrustningsPaket));
                OnPropertyChanged(nameof(VisarEnskildUtrustning));

                ValtUtrustningPaket = null;
                ValdEnskildUtrustning = null;
                ValdUtrustningArtikelTyp = null;
                AntalUtrustningsPaket = null;

                _aktuelltUtrustningPris = null;
                OnPropertyChanged(nameof(UtrustningPrisText));
            }
        }

        public bool VisarUtrustningsPaket => ValdUtrustningTyp == "Utrustningspaket";
        public bool VisarEnskildUtrustning => ValdUtrustningTyp == "Enskild utrustning";

        public UtrustningPaket? ValtUtrustningPaket
        {
            get => _valtUtrustningPaket;
            set
            {
                if (_valtUtrustningPaket == value)
                    return;

                _valtUtrustningPaket = value;
                OnPropertyChanged();

                UppdateraAntalPaketAlternativ();
                UppdateraUtrustningPris();
            }
        }

        public UtrustningEntitet? ValdEnskildUtrustning
        {
            get => _valdEnskildUtrustning;
            set
            {
                if (_valdEnskildUtrustning == value)
                    return;

                _valdEnskildUtrustning = value;
                OnPropertyChanged();

                UppdateraUtrustningPris();
            }
        }

        public ArtikelTyp? ValdUtrustningArtikelTyp
        {
            get => _valdUtrustningArtikelTyp;
            set
            {
                if (_valdUtrustningArtikelTyp == value)
                    return;

                _valdUtrustningArtikelTyp = value;
                OnPropertyChanged();

                UppdateraUtrustningPris();
            }
        }

        public int? AntalUtrustningsPaket
        {
            get => _antalUtrustningsPaket;
            set
            {
                if (_antalUtrustningsPaket == value)
                    return;

                _antalUtrustningsPaket = value;
                OnPropertyChanged();

                UppdateraUtrustningPris();
            }
        }

        #endregion

        #region Utrustning - visning

        public string UtrustningPrisText =>
            _aktuelltUtrustningPris.HasValue
                ? $"{_aktuelltUtrustningPris.Value:N0} kr"
                : "0 kr";

        #endregion

        #region Utrustning - tillgänglighet

        private void UppdateraTillgängligUtrustning()
        {
            TillgängligaUtrustningsPaket.Clear();
            TillgängligaEnskildaUtrustningar.Clear();
            TillgängligaUtrustningArtikelTyper.Clear();

            if (StartDatum == null || SlutDatum == null || SlutDatum <= StartDatum)
                return;

            UtrustningController utrustningController = new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(StartDatum.Value, SlutDatum.Value);

            var artikelTyper = utrustningController.HamtaAllaArtikelTyper();

            var ledigaArtikelTypNummer = ledigaUtrustningar
                .Where(u => u.ArtikelTypNummer.HasValue)
                .Select(u => u.ArtikelTypNummer!.Value)
                .Distinct()
                .ToList();

            foreach (ArtikelTyp artikelTyp in artikelTyper)
            {
                if (ledigaArtikelTypNummer.Contains(artikelTyp.ArtikelTypNummer))
                    TillgängligaUtrustningArtikelTyper.Add(artikelTyp);
            }

            foreach (UtrustningEntitet utrustning in ledigaUtrustningar)
                TillgängligaEnskildaUtrustningar.Add(utrustning);

            foreach (UtrustningPaket paket in utrustningController.HamtaAllaUtrustningsPaket())
            {
                int maxAntal =
                    BeräknaMaxAntalPaket(paket, ledigaUtrustningar, utrustningController);

                if (maxAntal > 0)
                    TillgängligaUtrustningsPaket.Add(paket);
            }
        }

        private int BeräknaMaxAntalPaket(
            UtrustningPaket paket,
            List<UtrustningEntitet> ledigaUtrustningar,
            UtrustningController utrustningController)
        {
            var innehåll =
                utrustningController.HamtaPaketInnehall(paket.UtrustningPaketNummer);

            if (!innehåll.Any())
                return 0;

            int maxAntal = int.MaxValue;

            foreach (UtrustningPaketInnehåll rad in innehåll)
            {
                if (!rad.ArtikelTypNummer.HasValue || rad.Antal <= 0)
                    return 0;

                int antalLediga =
                    ledigaUtrustningar.Count(u =>
                        u.ArtikelTypNummer == rad.ArtikelTypNummer.Value);

                int möjligtAntal = antalLediga / rad.Antal;

                maxAntal = Math.Min(maxAntal, möjligtAntal);
            }

            return maxAntal == int.MaxValue ? 0 : maxAntal;
        }

        private void UppdateraAntalPaketAlternativ()
        {
            AntalUtrustningsPaketAlternativ.Clear();
            AntalUtrustningsPaket = null;

            if (ValtUtrustningPaket == null || StartDatum == null || SlutDatum == null)
                return;

            UtrustningController utrustningController = new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(StartDatum.Value, SlutDatum.Value);

            int maxAntal =
                BeräknaMaxAntalPaket(
                    ValtUtrustningPaket,
                    ledigaUtrustningar,
                    utrustningController);

            if (AntalPersonerTotalt.HasValue)
                maxAntal = Math.Min(maxAntal, AntalPersonerTotalt.Value);

            for (int i = 1; i <= maxAntal; i++)
                AntalUtrustningsPaketAlternativ.Add(i);
        }

        #endregion

        #region Utrustning - pris

        private void UppdateraUtrustningPris()
        {
            _aktuelltUtrustningPris = null;
            OnPropertyChanged(nameof(UtrustningPrisText));

            if (StartDatum == null || SlutDatum == null)
                return;

            int? artikelTypNummer = null;

            if (ValdUtrustningTyp == "Utrustningspaket" && ValtUtrustningPaket != null)
                artikelTypNummer = ValtUtrustningPaket.ArtikelTypNummer;

            if (ValdUtrustningTyp == "Enskild utrustning" && ValdUtrustningArtikelTyp != null)
                artikelTypNummer = ValdUtrustningArtikelTyp.ArtikelTypNummer;

            if (!artikelTypNummer.HasValue)
                return;

            PrisController prisController = new PrisController();

            Pris? pris =
                prisController.HämtaAktuelltPris(
                    artikelTypNummer.Value,
                    StartDatum.Value,
                    SlutDatum.Value);

            if (pris == null)
                return;

            if (ValdUtrustningTyp == "Utrustningspaket")
            {
                int antalPaket = AntalUtrustningsPaket ?? 1;
                _aktuelltUtrustningPris = pris.PrisBelopp * antalPaket;
            }
            else
            {
                _aktuelltUtrustningPris = pris.PrisBelopp;
            }

            OnPropertyChanged(nameof(UtrustningPrisText));
        }

        #endregion

        #region Utrustning - lägg till

        private void LäggTillUtrustning()
        {
            if (!ValideraDatum())
                return;

            if (string.IsNullOrWhiteSpace(ValdUtrustningTyp))
            {
                VisaMeddelande?.Invoke("Du måste välja hur utrustningen ska bokas.");
                return;
            }

            if (ValdUtrustningTyp == "Enskild utrustning")
            {
                LäggTillEnskildUtrustning();
                return;
            }

            if (ValdUtrustningTyp == "Utrustningspaket")
                LäggTillUtrustningsPaket();
        }

        private void LäggTillEnskildUtrustning()
        {
            if (ValdUtrustningArtikelTyp == null)
            {
                VisaMeddelande?.Invoke("Du måste välja vilken typ av utrustning du vill boka.");
                return;
            }

            UtrustningController utrustningController = new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    StartDatum!.Value,
                    SlutDatum!.Value);

            UtrustningEntitet? valdFysiskUtrustning = ledigaUtrustningar
                .Where(u =>
                    u.ArtikelTypNummer ==
                    ValdUtrustningArtikelTyp.ArtikelTypNummer)
                .OrderBy(u => Guid.NewGuid())
                .FirstOrDefault();

            if (valdFysiskUtrustning == null)
            {
                VisaMeddelande?.Invoke(
                    "Det finns ingen ledig utrustning av den valda typen.");

                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris =
                prisController.HämtaAktuelltPris(
                    ValdUtrustningArtikelTyp.ArtikelTypNummer,
                    StartDatum.Value,
                    SlutDatum.Value);

            if (pris == null)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för den valda utrustningen.");

                return;
            }

            UtrustningRad utrustningRad = new UtrustningRad
            {
                StartDatum = StartDatum.Value,
                SlutDatum = SlutDatum.Value,
                UtrustningBelopp = pris.PrisBelopp,
                UtrustningNummer = valdFysiskUtrustning.UtrustningNummer,
                UtrustningPaketNummer = null,
                UtrustningDisplayText = ValdUtrustningArtikelTyp.TypNamn,
                SenastUppdaterad = DateTime.Now
            };

            UtrustningRader.Add(utrustningRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            ValdUtrustningArtikelTyp = null;
            _aktuelltUtrustningPris = null;

            OnPropertyChanged(nameof(UtrustningPrisText));

            UppdateraTillgängligUtrustning();
        }

        private void LäggTillUtrustningsPaket()
        {
            if (ValtUtrustningPaket == null)
            {
                VisaMeddelande?.Invoke("Du måste välja ett utrustningspaket.");
                return;
            }

            if (AntalUtrustningsPaket == null || AntalUtrustningsPaket <= 0)
            {
                VisaMeddelande?.Invoke("Du måste välja antal paket.");
                return;
            }

            if (!ValtUtrustningPaket.ArtikelTypNummer.HasValue)
            {
                VisaMeddelande?.Invoke("Utrustningspaketet saknar artikeltyp.");
                return;
            }

            UtrustningController utrustningController = new UtrustningController();
            PrisController prisController = new PrisController();

            Pris? pris =
                prisController.HämtaAktuelltPris(
                    ValtUtrustningPaket.ArtikelTypNummer.Value,
                    StartDatum!.Value,
                    SlutDatum!.Value);

            if (pris == null)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för utrustningspaketet.");

                return;
            }

            List<UtrustningPaketInnehåll> innehåll =
                utrustningController.HamtaPaketInnehall(
                    ValtUtrustningPaket.UtrustningPaketNummer);

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    StartDatum.Value,
                    SlutDatum.Value);

            HashSet<string> användaNummer = new HashSet<string>();

            for (int paketIndex = 0;
                 paketIndex < AntalUtrustningsPaket.Value;
                 paketIndex++)
            {
                UtrustningRad utrustningRad = new UtrustningRad
                {
                    StartDatum = StartDatum.Value,
                    SlutDatum = SlutDatum.Value,
                    UtrustningBelopp = pris.PrisBelopp,
                    UtrustningPaketNummer = ValtUtrustningPaket.UtrustningPaketNummer,
                    UtrustningNummer = null,
                    UtrustningDisplayText = ValtUtrustningPaket.UtrustningPaketNamn,
                    SenastUppdaterad = DateTime.Now
                };

                foreach (UtrustningPaketInnehåll innehållRad in innehåll)
                {
                    if (!innehållRad.ArtikelTypNummer.HasValue)
                        continue;

                    var matchandeUtrustningar = ledigaUtrustningar
                        .Where(u =>
                            u.ArtikelTypNummer ==
                            innehållRad.ArtikelTypNummer.Value)
                        .Where(u =>
                            !användaNummer.Contains(
                                u.UtrustningNummer))
                        .Take(innehållRad.Antal)
                        .ToList();

                    if (matchandeUtrustningar.Count < innehållRad.Antal)
                    {
                        VisaMeddelande?.Invoke(
                            "Det finns inte tillräckligt med ledig fysisk utrustning för paketet.");

                        return;
                    }

                    foreach (UtrustningEntitet utrustning in matchandeUtrustningar)
                    {
                        utrustningRad.UtrustningPaketRader.Add(
                            new UtrustningPaketRad
                            {
                                UtrustningNummer = utrustning.UtrustningNummer
                            });

                        användaNummer.Add(utrustning.UtrustningNummer);
                    }
                }

                UtrustningRader.Add(utrustningRad);
            }

            OnPropertyChanged(nameof(TotalBeloppText));

            ValtUtrustningPaket = null;
            AntalUtrustningsPaket = null;
            _aktuelltUtrustningPris = null;

            OnPropertyChanged(nameof(UtrustningPrisText));

            UppdateraTillgängligUtrustning();
        }

        #endregion



        #region Totalbelopp

        public string TotalBeloppText
        {
            get
            {
                decimal logiTotal = LogiRader.Sum(x => x.LogiBelopp);
                decimal utrustningTotal = UtrustningRader.Sum(x => x.UtrustningBelopp);

                return $"{logiTotal + utrustningTotal:N0} kr";
            }
        }

        #endregion

        #region Skapa bokning

        private void SkapaBokning()
        {
            if (ValdKund == null)
            {
                VisaMeddelande?.Invoke("Du måste välja en kund.");
                return;
            }

            if (!ValideraDatum())
                return;

            Entitetslager.Bokning nyBokning = new Entitetslager.Bokning
            {
                KundNummer = ValdKund.KundNummer,
                BokningsDatum = DateTime.Now,
                Status = "Aktiv",
                LogiRader = LogiRader.ToList(),
                UtrustningRader = UtrustningRader.ToList()
            };

            BokningController bokningController = new BokningController();
            bokningController.SkapaBokning(nyBokning);

            string kundText = ValdKund switch
            {
                PrivatKund privatKund => privatKund.DisplayText,
                FöretagsKund företagsKund => företagsKund.DisplayText,
                _ => ValdKund.KundNummer.ToString()
            };

            BokningSparad?.Invoke(
                kundText,
                DateTime.Now,
                LogiRader.ToList(),
                UtrustningRader.ToList());
        }

        #endregion

        #region Validering

        private bool ValideraDatum()
        {
            if (StartDatum == null || SlutDatum == null)
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

        #endregion

        #region Återställ formulär

        public void ÅterställFormulär()
        {
            LogiRader.Clear();
            UtrustningRader.Clear();

            ValdKund = null;

            StartDatum = null;
            SlutDatum = null;

            AntalPersonerTotalt = null;

            ValdBoendeTyp = null;
            ValdLogiArtikelTyp = null;
            ValtLogi = null;
            AntalPersonerValtLogi = null;

            ValdUtrustningTyp = null;
            ValtUtrustningPaket = null;
            ValdEnskildUtrustning = null;
            ValdUtrustningArtikelTyp = null;
            AntalUtrustningsPaket = null;

            TillgängligaLogi.Clear();
            TillgängligaLogiArtikelTyper.Clear();
            AntalPersonerValtLogiAlternativ.Clear();

            TillgängligaUtrustningsPaket.Clear();
            TillgängligaEnskildaUtrustningar.Clear();
            TillgängligaUtrustningArtikelTyper.Clear();
            AntalUtrustningsPaketAlternativ.Clear();

            _aktuelltLogiPris = null;
            _aktuelltUtrustningPris = null;

            OnPropertyChanged(nameof(LogiPrisText));
            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(BoendePlaceringText));
            OnPropertyChanged(nameof(TotalBeloppText));
        }

        #endregion



        #region PropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region RelayCommand

        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Predicate<object?>? _canExecute;

            public RelayCommand(
                Action<object?> execute,
                Predicate<object?>? canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }

            public event EventHandler? CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}