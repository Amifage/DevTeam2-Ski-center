using Entitetslager;
using Servicelager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BokningEntitet = Entitetslager.Bokning;
using SkidlektionEntitet = Entitetslager.Skidlektion;
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
        private DateTime? _utrustningStartDatum;
        private DateTime? _utrustningSlutDatum;
        private int? _antalEnskildUtrustning;

        private bool _ärRedigeringsläge;

        #endregion

        #region Events

        public event Action<string>? VisaMeddelande;
        public event Action<int, string, DateTime, List<LogiRad>, List<UtrustningRad>, List<KonferensRad>, List<SkidlektionRad>>? BokningSparad;

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
        public ObservableCollection<int> AntalEnskildUtrustningAlternativ { get; }




        public ObservableCollection<Konferens> TillgängligaKonferenser { get; }
        public ObservableCollection<int> AntalKonferensDeltagareAlternativ { get; }
        public ObservableCollection<KonferensRad> KonferensRader { get; }



        public ObservableCollection<SkidlektionEntitet> TillgängligaSkidlektioner { get; }
        public ObservableCollection<int> AntalSkidlektionPersonerAlternativ { get; }
        public ObservableCollection<SkidlektionRad> SkidlektionRader { get; }

        #endregion

        #region Commands

        public ICommand LäggTillLogiCommand { get; }
        public ICommand TaBortLogiRadCommand { get; }


        public ICommand LäggTillUtrustningCommand { get; }
        public ICommand TaBortUtrustningRadCommand { get; }

        public ICommand LäggTillKonferensCommand { get; }
        public ICommand TaBortKonferensRadCommand { get; }


        public ICommand LäggTillSkidlektionCommand { get; }
        public ICommand TaBortSkidlektionRadCommand { get; }

        public ICommand SkapaBokningCommand { get; }
        public ICommand TillbakaCommand { get; }


        public ICommand VäxlaRedigeringslägeCommand { get; }
        public ICommand AvbokaBokningCommand { get; }
        public ICommand SökBokningCommand { get; }
        public ICommand SparaÄndringarCommand { get; }

        #endregion

        #region Konstruktor

        public BokningViewModel()
        {
            KundController kundController = new KundController();

            Kunder = new ObservableCollection<Kund>(kundController.HämtaAllaKunder());
            AntalPersonerAlternativ = new ObservableCollection<int>(Enumerable.Range(1, 50));

            #region Logi
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

            LäggTillLogiCommand = new RelayCommand(_ => LäggTillLogi());
            TaBortLogiRadCommand = new RelayCommand(parameter => TaBortLogiRad(parameter));
            #endregion

            #region Utrustning
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
            AntalEnskildUtrustningAlternativ = new ObservableCollection<int>();

            LäggTillUtrustningCommand = new RelayCommand(_ => LäggTillUtrustning());
            TaBortUtrustningRadCommand = new RelayCommand(parameter => TaBortUtrustningRad(parameter));
            #endregion

            #region Konferens

            TillgängligaKonferenser = new ObservableCollection<Konferens>();
            AntalKonferensDeltagareAlternativ = new ObservableCollection<int>();
            KonferensRader = new ObservableCollection<KonferensRad>();

            LäggTillKonferensCommand = new RelayCommand(_ => LäggTillKonferens());
            TaBortKonferensRadCommand = new RelayCommand(parameter => TaBortKonferensRad(parameter));

            #endregion

            #region Skilektion

            TillgängligaSkidlektioner = new ObservableCollection<SkidlektionEntitet>();
            AntalSkidlektionPersonerAlternativ = new ObservableCollection<int>();
            SkidlektionRader = new ObservableCollection<SkidlektionRad>();

            LäggTillSkidlektionCommand = new RelayCommand(_ => LäggTillSkidlektion());
            TaBortSkidlektionRadCommand = new RelayCommand(parameter => TaBortSkidlektionRad(parameter));

            #endregion

            #region Ändra/Ta bort bokning

            BokningarFörRedigering = new ObservableCollection<BokningEntitet>();
            VäxlaRedigeringslägeCommand = new RelayCommand(_ => VäxlaRedigeringsläge());
            SökBokningCommand = new RelayCommand(_ => SökBokningar());
            SparaÄndringarCommand = new RelayCommand(_ => SparaÄndringar());
            AvbokaBokningCommand = new RelayCommand(_ => TaBortBokning());

            #endregion

            SkapaBokningCommand = new RelayCommand(_ => SkapaBokning());
            TillbakaCommand = new RelayCommand(TillbakaTillMeny);
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
                OnPropertyChanged(nameof(KanVäljaAntalPersonerTotalt));
                OnPropertyChanged(nameof(KanVäljaBoendeTyp));


                if (UtrustningStartDatum == null)
                    UtrustningStartDatum = value;
                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();


                if (KonferensStartTid == null)
                    KonferensStartTid = value;
                
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
                OnPropertyChanged(nameof(KanVäljaAntalPersonerTotalt));
                OnPropertyChanged(nameof(KanVäljaBoendeTyp));


                if (UtrustningSlutDatum == null)
                    UtrustningSlutDatum = value;
                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();

                if (KonferensSlutTid == null)
                    KonferensSlutTid = value;
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

                OnPropertyChanged(nameof(KanVäljaAntalPersonerValtLogi));
            }
        }

        public bool KanVäljaAntalPersonerTotalt =>
                    StartDatum.HasValue &&
                    SlutDatum.HasValue &&
                    SlutDatum.Value > StartDatum.Value;

        public bool KanVäljaAntalPersonerValtLogi =>
                    ValtLogi != null &&
                    AntalPersonerValtLogiAlternativ.Any();

        public bool KanVäljaBoendeTyp =>
                    StartDatum.HasValue &&
                    SlutDatum.HasValue &&
                    SlutDatum.Value > StartDatum.Value;

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
            OnPropertyChanged(nameof(KanVäljaAntalPersonerValtLogi));

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

            OnPropertyChanged(nameof(KanVäljaAntalPersonerValtLogi));
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

        #region Logi - lägg till/ta bort
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

        private void TaBortLogiRad(object? parameter)
        {
            if (parameter is not LogiRad logiRad)
                return;

            LogiRader.Remove(logiRad);

            OnPropertyChanged(nameof(TotalBeloppText));

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

        public DateTime? UtrustningStartDatum
        {
            get => _utrustningStartDatum;
            set
            {
                if (_utrustningStartDatum == value)
                    return;

                _utrustningStartDatum = value;
                OnPropertyChanged();

                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();

                OnPropertyChanged(nameof(KanVäljaUtrustningsPaket));
                OnPropertyChanged(nameof(KanVäljaUtrustningTyp));
            }
        }

        public DateTime? UtrustningSlutDatum
        {
            get => _utrustningSlutDatum;
            set
            {
                if (_utrustningSlutDatum == value)
                    return;

                _utrustningSlutDatum = value;
                OnPropertyChanged();

                UppdateraTillgängligUtrustning();
                UppdateraUtrustningPris();

                OnPropertyChanged(nameof(KanVäljaUtrustningsPaket));
                OnPropertyChanged(nameof(KanVäljaUtrustningTyp));
            }
        }

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
                OnPropertyChanged(nameof(KanLäggaTillUtrustning));

                UppdateraTillgängligUtrustning();
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

                OnPropertyChanged(nameof(KanVäljaAntalUtrustningsPaket));
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

                UppdateraAntalEnskildUtrustningAlternativ();
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

        public int? AntalEnskildUtrustning
        {
            get => _antalEnskildUtrustning;
            set
            {
                if (_antalEnskildUtrustning == value)
                    return;

                _antalEnskildUtrustning = value;
                OnPropertyChanged();

                UppdateraUtrustningPris();
            }
        }

        public bool KanVäljaUtrustningsPaket =>
                    UtrustningStartDatum.HasValue &&
                    UtrustningSlutDatum.HasValue &&
                    UtrustningSlutDatum.Value > UtrustningStartDatum.Value &&
                    TillgängligaUtrustningsPaket.Any();

        public bool KanVäljaUtrustningTyp =>
                    UtrustningStartDatum.HasValue &&
                    UtrustningSlutDatum.HasValue &&
                    UtrustningSlutDatum.Value > UtrustningStartDatum.Value;

        public bool KanVäljaAntalUtrustningsPaket =>
                    ValtUtrustningPaket != null &&
                    AntalUtrustningsPaketAlternativ.Any();

        #endregion

        #region Utrustning - visning

        public string UtrustningPrisText =>
             _aktuelltUtrustningPris.HasValue
                 ? $"{_aktuelltUtrustningPris.Value:N0} kr"
                 : "Pris saknas för vald period";

        public bool KanLäggaTillUtrustning =>
                    _aktuelltUtrustningPris.HasValue &&
                    (
                        (ValdUtrustningTyp == "Utrustningspaket" &&
                         ValtUtrustningPaket != null &&
                         AntalUtrustningsPaket.HasValue &&
                         AntalUtrustningsPaket.Value > 0)

                        ||

                        (ValdUtrustningTyp == "Enskild utrustning" &&
                         ValdUtrustningArtikelTyp != null &&
                         AntalEnskildUtrustning.HasValue &&
                         AntalEnskildUtrustning.Value > 0)
                    );
        #endregion

        #region Utrustning - tillgänglighet

        private void UppdateraTillgängligUtrustning()
        {
            TillgängligaUtrustningsPaket.Clear();
            TillgängligaEnskildaUtrustningar.Clear();
            TillgängligaUtrustningArtikelTyper.Clear();

            OnPropertyChanged(nameof(KanVäljaUtrustningsPaket));

            if (UtrustningStartDatum == null || UtrustningSlutDatum == null || UtrustningSlutDatum <= UtrustningStartDatum)
            {
                return;
            }

            UtrustningController utrustningController = new UtrustningController();

            var ledigaUtrustningar = utrustningController.HamtaLedigaUtrustningar(UtrustningStartDatum.Value, UtrustningSlutDatum.Value);

            var redanValdaNummer = UtrustningRader
                .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                .Select(r => r.UtrustningNummer!)
                .Concat(
                    UtrustningRader
                        .SelectMany(r => r.UtrustningPaketRader)
                        .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                        .Select(r => r.UtrustningNummer!)
                )
                .ToHashSet();

            ledigaUtrustningar = ledigaUtrustningar
                .Where(u => !redanValdaNummer.Contains(u.UtrustningNummer))
                .ToList();

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

            OnPropertyChanged(nameof(KanVäljaUtrustningsPaket));
        }

        private int BeräknaMaxAntalPaket(UtrustningPaket paket, List<UtrustningEntitet> ledigaUtrustningar, UtrustningController utrustningController)
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

        private void UppdateraAntalPaketAlternativ() //Skapar max antal möjliga paket utifrån ledig utrustning så man vet hur många paket som är bokningsbara
        {
            AntalUtrustningsPaketAlternativ.Clear();

            OnPropertyChanged(nameof(KanVäljaAntalUtrustningsPaket));

            AntalUtrustningsPaket = null;

            if (ValtUtrustningPaket == null || UtrustningStartDatum == null || UtrustningSlutDatum == null)
                return;

            UtrustningController utrustningController = new UtrustningController();

            var ledigaUtrustningar = utrustningController.HamtaLedigaUtrustningar(UtrustningStartDatum.Value, UtrustningSlutDatum.Value);

            var redanValdaNummer = UtrustningRader
                .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                .Select(r => r.UtrustningNummer!)
                .Concat(
                    UtrustningRader
                        .SelectMany(r => r.UtrustningPaketRader)
                        .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                        .Select(r => r.UtrustningNummer!)
                )
                .ToHashSet();

            ledigaUtrustningar = ledigaUtrustningar
                .Where(u => !redanValdaNummer.Contains(u.UtrustningNummer))
                .ToList();

            int maxAntal =
                BeräknaMaxAntalPaket(
                    ValtUtrustningPaket,
                    ledigaUtrustningar,
                    utrustningController);

            for (int i = 1; i <= maxAntal; i++)
                AntalUtrustningsPaketAlternativ.Add(i);

            OnPropertyChanged(nameof(KanVäljaAntalUtrustningsPaket));
        }

        private void UppdateraAntalEnskildUtrustningAlternativ()
        {
            AntalEnskildUtrustningAlternativ.Clear();
            AntalEnskildUtrustning = null;

            if (ValdUtrustningArtikelTyp == null ||
                UtrustningStartDatum == null ||
                UtrustningSlutDatum == null)
            {
                return;
            }

            UtrustningController utrustningController =
                new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    UtrustningStartDatum.Value,
                    UtrustningSlutDatum.Value);

            int antalLediga = ledigaUtrustningar
                .Where(u =>
                    u.ArtikelTypNummer ==
                    ValdUtrustningArtikelTyp.ArtikelTypNummer)
                .Where(u =>
                    !UtrustningRader.Any(r =>
                        r.UtrustningNummer == u.UtrustningNummer))
                .Count();

            for (int i = 1; i <= antalLediga; i++)
            {
                AntalEnskildUtrustningAlternativ.Add(i);
            }
        }
        #endregion

        #region Utrustning - pris

        private void UppdateraUtrustningPris()
        {
            _aktuelltUtrustningPris = null;
            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(KanLäggaTillUtrustning));

            if (UtrustningStartDatum == null || UtrustningSlutDatum == null || UtrustningSlutDatum <= UtrustningStartDatum)
            {
                _aktuelltUtrustningPris = null;
                OnPropertyChanged(nameof(UtrustningPrisText));
                OnPropertyChanged(nameof(KanLäggaTillUtrustning));
                return;
            }

            int? artikelTypNummer = null;

            if (ValdUtrustningTyp == "Utrustningspaket" && ValtUtrustningPaket != null)
                artikelTypNummer = ValtUtrustningPaket.ArtikelTypNummer;

            if (ValdUtrustningTyp == "Enskild utrustning" && ValdUtrustningArtikelTyp != null)
                artikelTypNummer = ValdUtrustningArtikelTyp.ArtikelTypNummer;

            if (!artikelTypNummer.HasValue)
                return;

            PrisController prisController = new PrisController();

            decimal? pris = prisController.BeräknaPrisFörPeriod(
                artikelTypNummer.Value,
                UtrustningStartDatum.Value,
                UtrustningSlutDatum.Value);

            if (!pris.HasValue)
            {
                _aktuelltUtrustningPris = null;
                OnPropertyChanged(nameof(UtrustningPrisText));
                OnPropertyChanged(nameof(KanLäggaTillUtrustning));
                return;
            }

            if (ValdUtrustningTyp == "Utrustningspaket")
            {
                int antalPaket = AntalUtrustningsPaket ?? 1;
                _aktuelltUtrustningPris = pris.Value * antalPaket;
            }
            else if (ValdUtrustningTyp == "Enskild utrustning")
            {
                if (!AntalEnskildUtrustning.HasValue)
                    return;

                _aktuelltUtrustningPris =
                    pris.Value * AntalEnskildUtrustning.Value;
            }

            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(KanLäggaTillUtrustning));
        }

        #endregion

        #region Utrustning - lägg till/ta bort

        private void LäggTillUtrustning()
        {
            if (!ValideraUtrustningsDatum())
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
            if (!ValideraUtrustningsDatum())
                return;

            if (ValdUtrustningArtikelTyp == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja vilken typ av utrustning du vill boka.");

                return;
            }

            if (!AntalEnskildUtrustning.HasValue ||
                AntalEnskildUtrustning.Value <= 0)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja antal.");

                return;
            }


            UtrustningController utrustningController =
                new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    UtrustningStartDatum!.Value,
                    UtrustningSlutDatum!.Value);


            // Hämta lediga fysiska exemplar av vald typ
            // och ta bort sådana som redan lagts till i den pågående bokningen.
            List<UtrustningEntitet> valdaFysiskaUtrustningar =
                ledigaUtrustningar
                    .Where(u =>
                        u.ArtikelTypNummer ==
                        ValdUtrustningArtikelTyp.ArtikelTypNummer)
                    .Where(u =>
                        !UtrustningRader.Any(r =>
                            r.UtrustningNummer ==
                            u.UtrustningNummer))
                    .Take(AntalEnskildUtrustning.Value)
                    .ToList();


            // Kontrollera att tillräckligt många exemplar finns
            if (valdaFysiskaUtrustningar.Count <
                AntalEnskildUtrustning.Value)
            {
                VisaMeddelande?.Invoke(
                    $"Det finns bara {valdaFysiskaUtrustningar.Count} lediga exemplar av den valda utrustningen.");

                return;
            }


            PrisController prisController =
                new PrisController();

            decimal? pris =
                prisController.BeräknaPrisFörPeriod(
                    ValdUtrustningArtikelTyp.ArtikelTypNummer,
                    UtrustningStartDatum.Value,
                    UtrustningSlutDatum.Value);


            if (pris == null)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för den valda utrustningen.");

                return;
            }


            // Skapa en separat UtrustningRad för varje fysiskt exemplar
            foreach (UtrustningEntitet fysiskUtrustning
                     in valdaFysiskaUtrustningar)
            {
                UtrustningRad utrustningRad =
                    new UtrustningRad
                    {
                        StartDatum =
                            UtrustningStartDatum.Value,

                        SlutDatum =
                            UtrustningSlutDatum.Value,

                        // Pris per fysisk utrustning
                        UtrustningBelopp =
                            pris.Value,

                        UtrustningNummer =
                            fysiskUtrustning.UtrustningNummer,

                        UtrustningPaketNummer =
                            null,

                        UtrustningDisplayText =
                            ValdUtrustningArtikelTyp.TypNamn,

                        SenastUppdaterad =
                            DateTime.Now
                    };

                UtrustningRader.Add(utrustningRad);
            }


            OnPropertyChanged(nameof(TotalBeloppText));


            // Uppdatera tillgängligheten efter att utrustningen lagts till
            UppdateraTillgängligUtrustning();


            // Återställ valen
            ValdUtrustningArtikelTyp = null;
            AntalEnskildUtrustning = null;
            _aktuelltUtrustningPris = null;

            AntalEnskildUtrustningAlternativ.Clear();


            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(KanLäggaTillUtrustning));
        }

        private void LäggTillUtrustningsPaket()
        {
            if (!ValideraUtrustningsDatum())
                return;


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

            decimal? pris = prisController.BeräknaPrisFörPeriod(
                ValtUtrustningPaket.ArtikelTypNummer.Value,
                UtrustningStartDatum.Value,
                UtrustningSlutDatum.Value);

            if (!pris.HasValue)
            {
                VisaMeddelande?.Invoke("Kunde inte hitta något pris för utrustningspaketet.");
                return;
            }

            List<UtrustningPaketInnehåll> innehåll =
                utrustningController.HamtaPaketInnehall(
                    ValtUtrustningPaket.UtrustningPaketNummer);

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    UtrustningStartDatum!.Value,
                    UtrustningSlutDatum!.Value);

            var redanValdaNummer = UtrustningRader
                .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                .Select(r => r.UtrustningNummer!)
                .Concat(
                    UtrustningRader
                        .SelectMany(r => r.UtrustningPaketRader)
                        .Where(r => !string.IsNullOrWhiteSpace(r.UtrustningNummer))
                        .Select(r => r.UtrustningNummer!)
                )
                .ToHashSet();

            ledigaUtrustningar = ledigaUtrustningar
                .Where(u => !redanValdaNummer.Contains(u.UtrustningNummer))
                .ToList();

            HashSet<string> användaNummer = new HashSet<string>();

            for (int paketIndex = 0;
                 paketIndex < AntalUtrustningsPaket.Value;
                 paketIndex++)
            {
                int antalPaket = AntalUtrustningsPaket ?? 1;

                UtrustningRad utrustningRad = new UtrustningRad
                {
                    StartDatum = UtrustningStartDatum.Value,
                    SlutDatum = UtrustningSlutDatum.Value,
                    UtrustningBelopp = pris.Value * antalPaket,
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

            UppdateraAntalPaketAlternativ();
            UppdateraTillgängligUtrustning();

            ValtUtrustningPaket = null;
            AntalUtrustningsPaket = null;
            _aktuelltUtrustningPris = null;

            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(KanLäggaTillUtrustning));
        }

        private void TaBortUtrustningRad(object? parameter)
        {
            if (parameter is not UtrustningRad utrustningRad)
                return;

            UtrustningRader.Remove(utrustningRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            UppdateraTillgängligUtrustning();
        }

        #endregion



        #region Konferens - val

        private DateTime? _konferensStartTid;
        private DateTime? _konferensSlutTid;
        private Konferens? _valdKonferens;
        private int? _antalKonferensDeltagare;
        private decimal? _aktuelltKonferensPris;

        public DateTime? KonferensStartTid
        {
            get => _konferensStartTid;
            set
            {
                if (_konferensStartTid == value)
                    return;

                _konferensStartTid = value;
                OnPropertyChanged();

                UppdateraTillgängligaKonferenser();
                UppdateraKonferensPris();
            }
        }

        public DateTime? KonferensSlutTid
        {
            get => _konferensSlutTid;
            set
            {
                if (_konferensSlutTid == value)
                    return;

                _konferensSlutTid = value;
                OnPropertyChanged();

                UppdateraTillgängligaKonferenser();
                UppdateraKonferensPris();
            }
        }

        public Konferens? ValdKonferens
        {
            get => _valdKonferens;
            set
            {
                if (_valdKonferens == value)
                    return;

                _valdKonferens = value;
                OnPropertyChanged();

                UppdateraAntalKonferensDeltagare();
                UppdateraKonferensPris();
            }
        }

        public int? AntalKonferensDeltagare
        {
            get => _antalKonferensDeltagare;
            set
            {
                if (_antalKonferensDeltagare == value)
                    return;

                _antalKonferensDeltagare = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(KanLäggaTillKonferens));
            }
        }

        public bool KanVäljaAntalKonferensDeltagare =>
                    ValdKonferens != null &&
                    AntalKonferensDeltagareAlternativ.Any();

        public bool KanVäljaKonferens =>
                    KonferensStartTid.HasValue &&
                    KonferensSlutTid.HasValue &&
                    TillgängligaKonferenser.Any();

        #endregion

        #region Konferens - visning

        public string KonferensPrisText =>
            _aktuelltKonferensPris.HasValue
                ? $"{_aktuelltKonferensPris.Value:N0} kr"
                : "Pris saknas";

        public bool KanLäggaTillKonferens =>
            ValdKonferens != null &&
            AntalKonferensDeltagare.HasValue &&
            _aktuelltKonferensPris.HasValue;

        #endregion

        #region Konferens - tillgänglighet

        private void UppdateraTillgängligaKonferenser()
        {
            TillgängligaKonferenser.Clear();

            OnPropertyChanged(nameof(KanVäljaKonferens));

            ValdKonferens = null;

            if (KonferensStartTid == null ||
                KonferensSlutTid == null ||
                KonferensSlutTid <= KonferensStartTid)
            {
                return;
            }

            KonferensController konferensController = new KonferensController();

            var ledigaKonferenser = konferensController.HamtaLedigaKonferenser(
                KonferensStartTid.Value,
                KonferensSlutTid.Value);

            var redanValdaKonferensNummer = KonferensRader
                .Where(r => !string.IsNullOrWhiteSpace(r.KonferensNummer))
                .Select(r => r.KonferensNummer!)
                .ToHashSet();

            ledigaKonferenser = ledigaKonferenser
                .Where(k => !redanValdaKonferensNummer.Contains(k.KonferensNummer))
                .OrderBy(k => k.KonferensKapacitet)
                .ToList();

            foreach (Konferens konferens in ledigaKonferenser)
                TillgängligaKonferenser.Add(konferens);

            OnPropertyChanged(nameof(KanVäljaKonferens));
        }

        private void UppdateraAntalKonferensDeltagare()
        {
            AntalKonferensDeltagareAlternativ.Clear();

            OnPropertyChanged(nameof(KanVäljaAntalKonferensDeltagare));

            AntalKonferensDeltagare = null;

            if (ValdKonferens == null)
                return;

            for (int i = 1; i <= ValdKonferens.KonferensKapacitet; i++)
                AntalKonferensDeltagareAlternativ.Add(i);

            OnPropertyChanged(nameof(KanVäljaAntalKonferensDeltagare));
        }

        #endregion

        #region Konferens - pris

        private void UppdateraKonferensPris()
        {
            _aktuelltKonferensPris = null;

            OnPropertyChanged(nameof(KonferensPrisText));
            OnPropertyChanged(nameof(KanLäggaTillKonferens));

            if (ValdKonferens?.ArtikelTypNummer == null ||
                KonferensStartTid == null ||
                KonferensSlutTid == null ||
                KonferensSlutTid <= KonferensStartTid)
            {
                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaAktuelltPris(
                ValdKonferens.ArtikelTypNummer.Value,
                KonferensStartTid.Value,
                KonferensSlutTid.Value);

            if (pris == null)
                return;

            _aktuelltKonferensPris = pris.PrisBelopp;

            OnPropertyChanged(nameof(KonferensPrisText));
            OnPropertyChanged(nameof(KanLäggaTillKonferens));
        }

        #endregion

        #region Konferens - lägg till/ta bort

        private void LäggTillKonferens()
        {
            if (!ValideraKonferensDatum())
                return;

            if (ValdKonferens == null)
            {
                VisaMeddelande?.Invoke("Du måste välja ett konferensrum.");
                return;
            }

            if (!AntalKonferensDeltagare.HasValue ||
                AntalKonferensDeltagare.Value <= 0)
            {
                VisaMeddelande?.Invoke("Du måste ange antal deltagare.");
                return;
            }

            if (AntalKonferensDeltagare.Value > ValdKonferens.KonferensKapacitet)
            {
                VisaMeddelande?.Invoke(
                    $"Konferensrummet rymmer maximalt {ValdKonferens.KonferensKapacitet} personer.");

                return;
            }

            if (!_aktuelltKonferensPris.HasValue)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för det valda konferensrummet.");

                return;
            }

            KonferensRad konferensRad = new KonferensRad
            {
                StartTid = KonferensStartTid!.Value,
                SlutTid = KonferensSlutTid!.Value,
                AntalPersoner = AntalKonferensDeltagare.Value,
                KonferensBelopp = _aktuelltKonferensPris.Value,
                KonferensNummer = ValdKonferens.KonferensNummer,
                SenastUppdaterad = DateTime.Now
            };

            KonferensRader.Add(konferensRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            UppdateraTillgängligaKonferenser();

            ValdKonferens = null;
            AntalKonferensDeltagare = null;

            _aktuelltKonferensPris = null;

            OnPropertyChanged(nameof(KonferensPrisText));
            OnPropertyChanged(nameof(KanLäggaTillKonferens));
        }

        private bool ValideraKonferensDatum()
        {
            if (KonferensStartTid == null || KonferensSlutTid == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja både startdatum och slutdatum för konferensen.");

                return false;
            }

            if (KonferensSlutTid <= KonferensStartTid)
            {
                VisaMeddelande?.Invoke(
                    "Konferensens slutdatum måste vara efter startdatum.");

                return false;
            }

            return true;
        }

        private void TaBortKonferensRad(object? parameter)
        {
            if (parameter is not KonferensRad konferensRad)
                return;

            KonferensRader.Remove(konferensRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            UppdateraTillgängligaKonferenser();
        }
        #endregion



        #region Skidlektion - val

        private SkidlektionEntitet? _valdSkidlektion;
        private int? _antalSkidlektionPersoner;
        private decimal? _aktuelltSkidlektionPris;

        public SkidlektionEntitet? ValdSkidlektion
        {
            get => _valdSkidlektion;
            set
            {
                if (_valdSkidlektion == value)
                    return;

                _valdSkidlektion = value;
                OnPropertyChanged();

                UppdateraAntalSkidlektionPersoner();
                UppdateraSkidlektionPris();
            }
        }

        public int? AntalSkidlektionPersoner
        {
            get => _antalSkidlektionPersoner;
            set
            {
                if (_antalSkidlektionPersoner == value)
                    return;

                _antalSkidlektionPersoner = value;
                OnPropertyChanged();

                UppdateraSkidlektionPris();
                OnPropertyChanged(nameof(KanLäggaTillSkidlektion));
            }
        }

        private DateTime? _skidlektionDatum;

        public DateTime? SkidlektionDatum
        {
            get => _skidlektionDatum;
            set
            {
                if (_skidlektionDatum == value)
                    return;

                _skidlektionDatum = value;
                OnPropertyChanged();

                UppdateraTillgängligaSkidlektioner();
            }
        }

        #endregion

        #region Skidlektion - visning

        public string SkidlektionPrisText =>
            _aktuelltSkidlektionPris.HasValue
                ? $"{_aktuelltSkidlektionPris.Value:N0} kr"
                : "Pris saknas";

        public bool KanLäggaTillSkidlektion =>
            ValdSkidlektion != null &&
            AntalSkidlektionPersoner.HasValue &&
            AntalSkidlektionPersoner.Value > 0 &&
            _aktuelltSkidlektionPris.HasValue;

        public bool VisaIngaTillgängligaSkidlektioner =>
            SkidlektionDatum.HasValue &&
            !TillgängligaSkidlektioner.Any();

        public bool FinnsTillgängligaSkidlektioner =>
            TillgängligaSkidlektioner.Any();

        public bool KanVäljaAntalSkidlektionPersoner =>
            ValdSkidlektion != null &&
            AntalSkidlektionPersonerAlternativ.Any();

        #endregion

        #region Skidlektion - tillgänglighet

        private void UppdateraTillgängligaSkidlektioner()
        {
            TillgängligaSkidlektioner.Clear();

            OnPropertyChanged(nameof(FinnsTillgängligaSkidlektioner));
            OnPropertyChanged(nameof(VisaIngaTillgängligaSkidlektioner));

            ValdSkidlektion = null;

            if (SkidlektionDatum == null)
                return;

            SkidlektionController skidlektionController =
                new SkidlektionController();

            var skidlektioner =
                skidlektionController.HämtaTillgängligaSkidlektioner(
                    SkidlektionDatum.Value);

            foreach (SkidlektionEntitet skidlektion in skidlektioner)
            {
                int redanValdaPersoner = SkidlektionRader
                    .Where(r => r.SkidlektionNummer == skidlektion.SkidlektionNummer)
                    .Sum(r => r.AntalPersoner);

                skidlektion.LedigaPlatser -= redanValdaPersoner;

                if (skidlektion.LedigaPlatser > 0)
                    TillgängligaSkidlektioner.Add(skidlektion);
            }

            OnPropertyChanged(nameof(VisaIngaTillgängligaSkidlektioner));
            OnPropertyChanged(nameof(FinnsTillgängligaSkidlektioner));
        }

        private void UppdateraAntalSkidlektionPersoner()
        {
            AntalSkidlektionPersonerAlternativ.Clear();

            OnPropertyChanged(nameof(KanVäljaAntalSkidlektionPersoner));

            AntalSkidlektionPersoner = null;

            if (ValdSkidlektion == null)
                return;

            int ledigaPlatser = ValdSkidlektion.LedigaPlatser;

            for (int i = 1; i <= ledigaPlatser; i++)
                AntalSkidlektionPersonerAlternativ.Add(i);
                OnPropertyChanged(nameof(KanVäljaAntalSkidlektionPersoner));
        }

        #endregion

        #region Skidlektion - pris

        private void UppdateraSkidlektionPris()
        {
            _aktuelltSkidlektionPris = null;

            OnPropertyChanged(nameof(SkidlektionPrisText));
            OnPropertyChanged(nameof(KanLäggaTillSkidlektion));

            if (ValdSkidlektion?.ArtikelTypNummer == null ||
                !AntalSkidlektionPersoner.HasValue)
            {
                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaPrisFörSkidlektion(
                        ValdSkidlektion.ArtikelTypNummer.Value,
                        ValdSkidlektion.Datum);

            if (pris == null)
                return;

            _aktuelltSkidlektionPris =
                pris.PrisBelopp * AntalSkidlektionPersoner.Value;

            OnPropertyChanged(nameof(SkidlektionPrisText));
            OnPropertyChanged(nameof(KanLäggaTillSkidlektion));
        }

        #endregion

        #region Skidlektion - lägg till/ta bort

        private void LäggTillSkidlektion()
        {
            if (ValdSkidlektion == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja en skidlektion.");

                return;
            }

            if (!AntalSkidlektionPersoner.HasValue ||
                AntalSkidlektionPersoner.Value <= 0)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja antal personer.");

                return;
            }

            SkidlektionController skidlektionController =
                new SkidlektionController();

            int ledigaPlatser =
                skidlektionController.HämtaAntalLedigaPlatser(
                    ValdSkidlektion.SkidlektionNummer);

            int redanValdaPersoner = SkidlektionRader
                .Where(r =>
                    r.SkidlektionNummer ==
                    ValdSkidlektion.SkidlektionNummer)
                .Sum(r => r.AntalPersoner);

            ledigaPlatser -= redanValdaPersoner;

            if (AntalSkidlektionPersoner.Value > ledigaPlatser)
            {
                VisaMeddelande?.Invoke(
                    $"Det finns bara {ledigaPlatser} lediga platser på skidlektionen.");

                return;
            }

            if (!_aktuelltSkidlektionPris.HasValue)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för den valda skidlektionen.");

                return;
            }

            SkidlektionRad skidlektionRad = new SkidlektionRad
            {
                Datum = ValdSkidlektion.Datum,
                AntalPersoner = AntalSkidlektionPersoner.Value,
                SkidlektionBelopp = _aktuelltSkidlektionPris.Value,
                SkidlektionNummer = ValdSkidlektion.SkidlektionNummer,
                SenastUppdaterad = DateTime.Now,

                SkidlektionDisplayText =
                $"{ValdSkidlektion.artikelTyp?.TypNamn ?? "Skidlektion"} | " +
                $"{ValdSkidlektion.StartTid:hh\\:mm} - {ValdSkidlektion.SlutTid:hh\\:mm}",
            };

            SkidlektionRader.Add(skidlektionRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            UppdateraTillgängligaSkidlektioner();

            ValdSkidlektion = null;
            AntalSkidlektionPersoner = null;

            _aktuelltSkidlektionPris = null;

            OnPropertyChanged(nameof(SkidlektionPrisText));
            OnPropertyChanged(nameof(KanLäggaTillSkidlektion));
        }
        private void TaBortSkidlektionRad(object? parameter)
        {
            if (parameter is not SkidlektionRad skidlektionRad)
                return;

            SkidlektionRader.Remove(skidlektionRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            UppdateraTillgängligaSkidlektioner();
        }
        #endregion



        #region Ändra / ta bort bokning

        private string? _bokningSökText;
        private BokningEntitet? _valdBokningFörRedigering;


        public bool ÄrRedigeringsläge
        {
            get => _ärRedigeringsläge;
            set
            {
                if (_ärRedigeringsläge == value)
                    return;

                _ärRedigeringsläge = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(BokningsRubrik));
            }
        }


        public string? BokningSökText
        {
            get => _bokningSökText;
            set
            {
                if (_bokningSökText == value)
                    return;

                _bokningSökText = value;
                OnPropertyChanged();
            }
        }


        public BokningEntitet? ValdBokningFörRedigering
        {
            get => _valdBokningFörRedigering;
            set
            {
                if (_valdBokningFörRedigering == value)
                    return;

                _valdBokningFörRedigering = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BokningsRubrik));

                if (_valdBokningFörRedigering != null)
                {
                    LaddaBokningFörRedigering(
                        _valdBokningFörRedigering);
                }
            }
        }


        public ObservableCollection<BokningEntitet>
            BokningarFörRedigering
        { get; }

        public string BokningsRubrik =>
        ÄrRedigeringsläge && ValdBokningFörRedigering != null
            ? $"Redigerar bokning {ValdBokningFörRedigering.BokningsNummer}"
            : "Skapa bokning";

        private void VäxlaRedigeringsläge()
        {
            ÄrRedigeringsläge = !ÄrRedigeringsläge;

            if (ÄrRedigeringsläge)
            {
                BokningSökText = null;
                SökBokningar();
            }
            else
            {
                AvslutaRedigeringsläge();
            }
        }


        private void AvslutaRedigeringsläge()
        {
            BokningSökText = null;
            ValdBokningFörRedigering = null;

            BokningarFörRedigering.Clear();

            ÅterställFormulär();
        }


        private void SökBokningar()
        {
            BokningarFörRedigering.Clear();

            BokningController bokningController =
                new BokningController();

            List<BokningEntitet> bokningar =
                bokningController.SökBokningar(BokningSökText);

            foreach (BokningEntitet bokning in bokningar)
            {
                BokningarFörRedigering.Add(bokning);
            }
        }


        private void LaddaBokningFörRedigering(BokningEntitet bokning)
        {
            BokningController bokningController =
                new BokningController();

            BokningEntitet? fullständigBokning =
                bokningController.HämtaBokningMedRader(
                    bokning.BokningsNummer);

            if (fullständigBokning == null)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta den valda bokningen.");

                return;
            }


            // Kund

            ValdKund = Kunder
                .FirstOrDefault(k =>
                    k.KundNummer == fullständigBokning.KundNummer);


            // Töm nuvarande rader

            LogiRader.Clear();
            UtrustningRader.Clear();
            KonferensRader.Clear();
            SkidlektionRader.Clear();


            // Ladda boende

            if (fullständigBokning.LogiRader != null)
            {
                foreach (LogiRad rad in fullständigBokning.LogiRader)
                {
                    LogiRader.Add(rad);
                }
            }


            // Ladda utrustning

            if (fullständigBokning.UtrustningRader != null)
            {
                foreach (UtrustningRad rad in fullständigBokning.UtrustningRader)
                {
                    UtrustningRader.Add(rad);
                }
            }


            // Ladda konferens

            if (fullständigBokning.KonferensRader != null)
            {
                foreach (KonferensRad rad in fullständigBokning.KonferensRader)
                {
                    KonferensRader.Add(rad);
                }
            }


            // Ladda skidlektion

            if (fullständigBokning.SkidlektionRader != null)
            {
                foreach (SkidlektionRad rad in fullständigBokning.SkidlektionRader)
                {
                    SkidlektionRader.Add(rad);
                }
            }


            OnPropertyChanged(nameof(TotalBeloppText));
        }

        private void SparaÄndringar()
        {
            if (ValdBokningFörRedigering == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja en bokning att ändra.");

                return;
            }

            if (ValdKund == null)
            {
                VisaMeddelande?.Invoke(
                    "Bokningen måste ha en kund.");

                return;
            }

            try
            {
                BokningController bokningController =
                    new BokningController();

                BokningEntitet? gammalBokning =
                bokningController.HämtaBokningMedRader(
                    ValdBokningFörRedigering.BokningsNummer);

                decimal tidigareTotalBelopp = 0;

                if (gammalBokning != null)
                {
                    tidigareTotalBelopp =
                        (gammalBokning.LogiRader?.Sum(x => x.LogiBelopp) ?? 0) +
                        (gammalBokning.UtrustningRader?.Sum(x => x.UtrustningBelopp) ?? 0) +
                        (gammalBokning.KonferensRader?.Sum(x => x.KonferensBelopp) ?? 0) +
                        (gammalBokning.SkidlektionRader?.Sum(x => x.SkidlektionBelopp) ?? 0);
                }

                bokningController.SparaÄndringar(
                    ValdBokningFörRedigering.BokningsNummer,
                    ValdKund.KundNummer,
                    LogiRader.ToList(),
                    UtrustningRader.ToList(),
                    KonferensRader.ToList(),
                    SkidlektionRader.ToList());


                Ändringsbekräftelse ändringsbekräftelse =
                 new Ändringsbekräftelse(
                     ValdBokningFörRedigering.BokningsNummer,
                     $"{ValdKund.VisningsNamn}|{ValdKund.KundNummer}",
                     DateTime.Now,
                     tidigareTotalBelopp,
                     LogiRader.ToList(),
                     UtrustningRader.ToList(),
                     KonferensRader.ToList(),
                     SkidlektionRader.ToList());

                ändringsbekräftelse.ShowDialog();


                int bokningsNummer =
                    ValdBokningFörRedigering.BokningsNummer;


                // Uppdatera listan till höger
                SökBokningar();


                // Ladda om bokningen så att vi får de nya
                // radnumren från databasen
                BokningEntitet? bokningAttLadda =
                    BokningarFörRedigering
                        .FirstOrDefault(b =>
                            b.BokningsNummer == bokningsNummer);

                if (bokningAttLadda != null)
                {
                    ValdBokningFörRedigering =
                        bokningAttLadda;
                }
            }
            catch (Exception ex)
            {
                string felmeddelande = ex.Message;

                if (ex.InnerException != null)
                {
                    felmeddelande +=
                        $"\n\nInner exception:\n{ex.InnerException.Message}";
                }

                if (ex.InnerException?.InnerException != null)
                {
                    felmeddelande +=
                        $"\n\nInner inner exception:\n{ex.InnerException.InnerException.Message}";
                }

                VisaMeddelande?.Invoke(
                    $"Kunde inte spara ändringarna.\n\n{felmeddelande}");
            }
        }

        private void TaBortBokning()
        {
            if (ValdBokningFörRedigering == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja en bokning att avboka.");

                return;
            }

            int bokningsNummer =
                ValdBokningFörRedigering.BokningsNummer;

            MessageBoxResult svar = MessageBox.Show(
                $"Är du säker på att du vill avboka bokning {bokningsNummer}?",
                "Bekräfta avbokning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (svar != MessageBoxResult.Yes)
                return;


            List<LogiRad> avbokadeLogiRader = LogiRader.ToList();

            List<UtrustningRad> avbokadeUtrustningRader =
                UtrustningRader.ToList();

            List<KonferensRad> avbokadeKonferensRader =
                KonferensRader.ToList();

            List<SkidlektionRad> avbokadeSkidlektionRader =
                SkidlektionRader.ToList();

            string kundText = ValdKund != null
                ? $"{ValdKund.VisningsNamn}|{ValdKund.KundNummer}"
                : "Okänd kund|-";


            try
            {
                BokningController bokningController =
                    new BokningController();

                bokningController.TaBortBokning(
                    bokningsNummer);

                Avbokningsbekräftelse avbokningsbekräftelse =
                new Avbokningsbekräftelse(
                    bokningsNummer,
                    kundText,
                    DateTime.Now,
                    avbokadeLogiRader,
                    avbokadeUtrustningRader,
                    avbokadeKonferensRader,
                    avbokadeSkidlektionRader);

                avbokningsbekräftelse.ShowDialog();

                ValdBokningFörRedigering = null;

                LogiRader.Clear();
                UtrustningRader.Clear();
                KonferensRader.Clear();
                SkidlektionRader.Clear();

                ValdKund = null;
                StartDatum = null;
                SlutDatum = null;

                OnPropertyChanged(nameof(TotalBeloppText));

                SökBokningar();
            }
            catch (Exception ex)
            {
                string felmeddelande = ex.Message;

                if (ex.InnerException != null)
                {
                    felmeddelande +=
                        $"\n\nInner exception:\n{ex.InnerException.Message}";
                }

                VisaMeddelande?.Invoke(
                    $"Kunde inte ta bort bokningen.\n\n{felmeddelande}");
            }
        }


        #endregion



        #region Totalbelopp

        public string TotalBeloppText =>
                    $"{LogiRader.Sum(x => x.LogiBelopp) +
                       UtrustningRader.Sum(x => x.UtrustningBelopp) +
                       KonferensRader.Sum(x => x.KonferensBelopp) +
                       SkidlektionRader.Sum(x => x.SkidlektionBelopp):N0} kr";

        #endregion

        #region Skapa bokning

        private void SkapaBokning()
        {
            if (ValdKund == null)
            {
                VisaMeddelande?.Invoke("Du måste välja en kund.");
                return;
            }

            if (LogiRader.Any() && !ValideraDatum())
                return;

            Entitetslager.Bokning nyBokning = new Entitetslager.Bokning
            {
                KundNummer = ValdKund.KundNummer,
                BokningsDatum = DateTime.Now,
                Status = "Aktiv",

                LogiRader = LogiRader.ToList(),
                UtrustningRader = UtrustningRader.ToList(),
                KonferensRader = KonferensRader.ToList(),
                SkidlektionRader = SkidlektionRader.ToList()
            };

            BokningController bokningController = new BokningController();
            bokningController.SkapaBokning(nyBokning);

            string kundText = $"{ValdKund.VisningsNamn}|{ValdKund.KundNummer}";

            BokningSparad?.Invoke(
                nyBokning.BokningsNummer, 
                kundText,
                DateTime.Now,
                LogiRader.ToList(),
                UtrustningRader.ToList(),
                KonferensRader.ToList(),
                SkidlektionRader.ToList());
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

        private bool ValideraUtrustningsDatum()
        {
            if (UtrustningStartDatum == null || UtrustningSlutDatum == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja både startdatum och slutdatum för utrustningen.");

                return false;
            }

            if (UtrustningSlutDatum <= UtrustningStartDatum)
            {
                VisaMeddelande?.Invoke(
                    "Utrustningens slutdatum måste vara efter startdatum.");

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
            KonferensRader.Clear();
            SkidlektionRader.Clear();

            ValdKund = null;
            StartDatum = null;
            SlutDatum = null;
            AntalPersonerTotalt = null;

            ValdBoendeTyp = null;
            ValdLogiArtikelTyp = null;
            ValtLogi = null;
            AntalPersonerValtLogi = null;
            TillgängligaLogi.Clear();
            TillgängligaLogiArtikelTyper.Clear();
            AntalPersonerValtLogiAlternativ.Clear();

            ValdUtrustningTyp = null;
            ValtUtrustningPaket = null;
            ValdEnskildUtrustning = null;
            ValdUtrustningArtikelTyp = null;
            AntalUtrustningsPaket = null;
            UtrustningStartDatum = null;
            UtrustningSlutDatum = null;
            TillgängligaUtrustningsPaket.Clear();
            TillgängligaEnskildaUtrustningar.Clear();
            TillgängligaUtrustningArtikelTyper.Clear();
            AntalUtrustningsPaketAlternativ.Clear();

            ValdKonferens = null;
            AntalKonferensDeltagare = null;
            KonferensStartTid = null;
            KonferensSlutTid = null;
            TillgängligaKonferenser.Clear();
            AntalKonferensDeltagareAlternativ.Clear();

            ValdSkidlektion = null;
            AntalSkidlektionPersoner = null;
            SkidlektionDatum = null;
            TillgängligaSkidlektioner.Clear();
            AntalSkidlektionPersonerAlternativ.Clear();

            _aktuelltLogiPris = null;
            _aktuelltUtrustningPris = null;
            _aktuelltKonferensPris = null;
            _aktuelltSkidlektionPris = null;

            OnPropertyChanged(nameof(LogiPrisText));
            OnPropertyChanged(nameof(UtrustningPrisText));
            OnPropertyChanged(nameof(KanLäggaTillUtrustning));
            OnPropertyChanged(nameof(BoendePlaceringText));
            OnPropertyChanged(nameof(KonferensPrisText));
            OnPropertyChanged(nameof(KanLäggaTillKonferens));
            OnPropertyChanged(nameof(SkidlektionPrisText));
            OnPropertyChanged(nameof(KanLäggaTillSkidlektion));
            OnPropertyChanged(nameof(TotalBeloppText));
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