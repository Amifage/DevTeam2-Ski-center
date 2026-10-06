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
        private Kund? _valdKund;
        private DateTime? _startDatum;
        private DateTime? _slutDatum;
        private int? _antalPersonerTotalt;

        private string? _valdBoendeTyp;
        private Logi? _valtLogi;
        private int? _antalPersonerValtLogi;

        private decimal? _aktuelltLogiPris;


        private string? _valdUtrustningTyp;
        private UtrustningPaket? _valtUtrustningPaket;
        public UtrustningEntitet? _valdEnskildUtrustning;
        private int? _antalUtrustningsPaket;
        private decimal? _aktuelltUtrustningPris;



        public event Action<string>? VisaMeddelande;

        public event Action<string, DateTime, List<LogiRad>, List<UtrustningRad>>? BokningSparad;

        public event Action? StängFönster;


        public ObservableCollection<Kund> Kunder { get; }

        public ObservableCollection<int> AntalPersonerAlternativ { get; }

        public ObservableCollection<string> BoendeTyper { get; }

        public ObservableCollection<Logi> TillgängligaLogi { get; }

        public ObservableCollection<int> AntalPersonerValtLogiAlternativ { get; }

        public ObservableCollection<LogiRad> LogiRader { get; }



        public ObservableCollection<string> UtrustningTyper { get; }

        public ObservableCollection<UtrustningPaket> TillgängligaUtrustningsPaket { get; }

        public ObservableCollection<UtrustningEntitet> TillgängligaEnskildaUtrustningar { get; }

        public ObservableCollection<int> AntalUtrustningsPaketAlternativ { get; }

        public ObservableCollection<UtrustningRad> UtrustningRader { get; }


        #region Vald kund

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
                UppdateraPris();
                UppdateraTillgängligUtrustning();
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
                UppdateraTillgängligUtrustning();
            }
        }
#endregion

        #region Antal Personer
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
        #endregion 

        #region Vald Logi
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
        #endregion

        #region Visningstext

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


        public string TotalBeloppText
        {
            get
            {
                decimal logiTotal =
                    LogiRader.Sum(x => x.LogiBelopp);

                decimal utrustningTotal =
                    UtrustningRader.Sum(x => x.UtrustningBelopp);

                decimal total =
                    logiTotal + utrustningTotal;

                return $"{total:N0} kr";
            }
        }
        #endregion

        public ICommand LäggTillLogiCommand { get; }
        public ICommand SkapaBokningCommand { get; }
        public ICommand AvbrytCommand { get; }

        public ICommand LäggTillUtrustningCommand { get; }

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
            AntalPersonerValtLogiAlternativ = new ObservableCollection<int>();
            LogiRader = new ObservableCollection<LogiRad>();
            LäggTillLogiCommand = new RelayCommand(_ => LäggTillLogi());
            SkapaBokningCommand = new RelayCommand(_ => SkapaBokning());
            AvbrytCommand = new RelayCommand(_ => StängFönster?.Invoke());


            UtrustningTyper = new ObservableCollection<string>
            {
                "Utrustningspaket",
                "Enskild utrustning"
            };

            TillgängligaUtrustningsPaket =
                new ObservableCollection<UtrustningPaket>();

            TillgängligaEnskildaUtrustningar = new ObservableCollection<UtrustningEntitet>();

            AntalUtrustningsPaketAlternativ =
                new ObservableCollection<int>();

            UtrustningRader =
                new ObservableCollection<UtrustningRad>();


            LäggTillUtrustningCommand = new RelayCommand(_ => LäggTillUtrustning());
        }


        #region Logi

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


            LogiController logiController = new LogiController();
            var allaLogi = logiController.HämtaAllaLogi();
            var upptagnaLogi =logiController.HämtaUpptagnaLogi(StartDatum.Value, SlutDatum.Value);


            IEnumerable<Logi> filtreradeLogi;


            if (ValdBoendeTyp == "Lägenhet")
            {
                filtreradeLogi = allaLogi
                    .Where(l => l.ArtikelTypNummer == 1 || l.ArtikelTypNummer == 2)
                    .Where(l => !LogiRader.Any(r => r.LogiNummer == l.LogiNummer))
                    .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                    .OrderBy(l => AvståndTillValdaLogi(l))
                    .ThenBy(l => HämtaLogiNummer(l));
            }
            else
            {
                filtreradeLogi =  allaLogi
                         .Where(l => l.ArtikelTypNummer == 3)
                         .Where(l => !LogiRader.Any(r => r.LogiNummer ==  l.LogiNummer))
                         .Where(l => !upptagnaLogi.Contains(l.LogiNummer))
                         .OrderBy(l => HämtaLogiNummer(l));
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


            if (ValtLogi == null || AntalPersonerTotalt == null)
            {
                return;
            }

            int redanPlacerade = LogiRader.Sum(x => x.AntalPersoner);
            int återstående = AntalPersonerTotalt.Value -  redanPlacerade;
       
            if (återstående <= 0)
                return;

            int maxAntal;

            if (ValtLogi.LogiKapacitet.HasValue)
            {
                maxAntal = Math.Min(ValtLogi.LogiKapacitet.Value, återstående);
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

            OnPropertyChanged(nameof(LogiPrisText));


            if (ValtLogi == null || StartDatum == null || SlutDatum == null || ValtLogi.ArtikelTypNummer == null)
            {
                return;
            }

            PrisController prisController = new PrisController();

            Pris? pris = prisController.HämtaAktuelltPris(ValtLogi.ArtikelTypNummer.Value, StartDatum.Value,SlutDatum.Value);

            if (pris != null)
            {
                _aktuelltLogiPris = pris.PrisBelopp;
            }

            OnPropertyChanged(nameof(LogiPrisText));
        }

        private void LäggTillLogi()
        {
            if (AntalPersonerTotalt == null)
            {
                VisaMeddelande?.Invoke("Du måste välja totalt antal personer.");

                return;
            }

            if (ValtLogi == null)
            {
                VisaMeddelande?.Invoke( "Du måste välja ett boende.");

                return;
            }

            if (AntalPersonerValtLogi == null)
            {
                VisaMeddelande?.Invoke( "Du måste välja antal personer för boendet.");

                return;
            }

            if (!ValideraDatum())
                return;

            int redanPlacerade =LogiRader.Sum(x =>  x.AntalPersoner);

            if (redanPlacerade + AntalPersonerValtLogi.Value > AntalPersonerTotalt.Value)
            {
                VisaMeddelande?.Invoke("Du kan inte placera fler personer än det totala antalet i bokningen.");

                return;
            }

            UppdateraPris();

            if (!_aktuelltLogiPris.HasValue)
            {
                VisaMeddelande?.Invoke( "Kunde inte hitta något pris för det valda boendet.");

                return;
            }

            LogiRad logiRad = new LogiRad
                {
                    StartDatum = StartDatum!.Value,
                    SlutDatum = SlutDatum!.Value,
                    LogiBelopp = _aktuelltLogiPris.Value,
                    AntalPersoner = AntalPersonerValtLogi.Value,
                    LogiNummer = ValtLogi.LogiNummer,
                    LogiDisplayText = ValtLogi.DisplayText,
                    SenastUppdaterad = DateTime.Now
                };


            LogiRader.Add(logiRad);

            OnPropertyChanged(nameof(BoendePlaceringText));

            OnPropertyChanged(nameof(TotalBeloppText));


            ValtLogi = null;

            _aktuelltLogiPris = null;

            OnPropertyChanged( nameof(LogiPrisText));


            UppdateraTillgängligaLogi();
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

            bokningController.SkapaBokning( nyBokning);

            string kundText = ValdKund switch
                {
                    PrivatKund privatKund => privatKund.DisplayText,

                    FöretagsKund företagsKund => företagsKund.DisplayText, _ => ValdKund.KundNummer.ToString()
                };


            BokningSparad?.Invoke(kundText, DateTime.Now, LogiRader.ToList(), UtrustningRader.ToList());
        }

        private bool ValideraDatum()
        {
            if (StartDatum == null || SlutDatum == null)
            {
                VisaMeddelande?.Invoke( "Du måste välja både startdatum och slutdatum.");

                return false;
            }

            if (SlutDatum < StartDatum)
            {
                VisaMeddelande?.Invoke( "Slutdatum kan inte vara före startdatum.");

                return false;
            }

            return true;
        }
#endregion 

        #region Återställ Formulär

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


            OnPropertyChanged( nameof(LogiPrisText));
            OnPropertyChanged( nameof(BoendePlaceringText));
            OnPropertyChanged(nameof(TotalBeloppText));
        }
#endregion

        #region Hjälpmetoder

        private int HämtaLogiNummer(Logi logi)
        {
            if (string.IsNullOrWhiteSpace( logi.LogiNummer))
            {
                return int.MaxValue;
            }

            string siffror = new string( logi.LogiNummer
                        .Where(char.IsDigit)
                        .ToArray());

            return int.TryParse( siffror, out int nummer) ? nummer : int.MaxValue;
        }

        private int AvståndTillValdaLogi( Logi logi)
        {
            if (!LogiRader.Any())
                return 0;

            int aktuelltNummer = HämtaLogiNummer(logi);

            return LogiRader
                .Where(r => !string.IsNullOrWhiteSpace( r.LogiNummer))

                .Select(r =>
                {
                    string siffror = new string(  r.LogiNummer!
                                .Where(char.IsDigit)
                                .ToArray());

                    return int.TryParse( siffror, out int nummer) ? Math.Abs( aktuelltNummer - nummer) : int.MaxValue;
                })

                .DefaultIfEmpty(0)
                .Min();
        }


        private void UppdateraTillgängligUtrustning()
        {
            TillgängligaUtrustningsPaket.Clear();
            TillgängligaEnskildaUtrustningar.Clear();

            if (StartDatum == null ||
                SlutDatum == null ||
                SlutDatum <= StartDatum)
            {
                return;
            }

            UtrustningController utrustningController =
                new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    StartDatum.Value,
                    SlutDatum.Value);

            foreach (UtrustningEntitet utrustning in ledigaUtrustningar)
            {
                TillgängligaEnskildaUtrustningar.Add(utrustning);
            }

            foreach (UtrustningPaket paket in utrustningController.HamtaAllaUtrustningsPaket())
            {
                int maxAntal =
                    BeräknaMaxAntalPaket(
                        paket,
                        ledigaUtrustningar,
                        utrustningController);

                if (maxAntal > 0)
                {
                    TillgängligaUtrustningsPaket.Add(paket);
                }
            }
        }

        private int BeräknaMaxAntalPaket(
            UtrustningPaket paket,
            List<UtrustningEntitet> ledigaUtrustningar,
            UtrustningController utrustningController)
        {
            var innehåll =
                utrustningController.HamtaPaketInnehall(
                    paket.UtrustningPaketNummer);

            if (!innehåll.Any())
            {
                return 0;
            }

            int maxAntal = int.MaxValue;

            foreach (UtrustningPaketInnehåll rad in innehåll)
            {
                if (!rad.ArtikelTypNummer.HasValue || rad.Antal <= 0)
                {
                    return 0;
                }

                int antalLediga =
                    ledigaUtrustningar.Count(u =>
                        u.ArtikelTypNummer == rad.ArtikelTypNummer.Value);

                int möjligtAntal =
                    antalLediga / rad.Antal;

                maxAntal =
                    Math.Min(maxAntal, möjligtAntal);
            }

            return maxAntal == int.MaxValue
                ? 0
                : maxAntal;
        }

        private void UppdateraAntalPaketAlternativ()
        {
            AntalUtrustningsPaketAlternativ.Clear();
            AntalUtrustningsPaket = null;

            if (ValtUtrustningPaket == null ||
                StartDatum == null ||
                SlutDatum == null)
            {
                return;
            }

            UtrustningController utrustningController =
                new UtrustningController();

            var ledigaUtrustningar =
                utrustningController.HamtaLedigaUtrustningar(
                    StartDatum.Value,
                    SlutDatum.Value);

            int maxAntal =
                BeräknaMaxAntalPaket(
                    ValtUtrustningPaket,
                    ledigaUtrustningar,
                    utrustningController);

            for (int i = 1; i <= maxAntal; i++)
            {
                AntalUtrustningsPaketAlternativ.Add(i);
            }
        }

        private void UppdateraUtrustningPris()
        {
            _aktuelltUtrustningPris = null;
            OnPropertyChanged(nameof(UtrustningPrisText));

            if (StartDatum == null ||
                SlutDatum == null)
            {
                return;
            }

            int? artikelTypNummer = null;

            if (ValdUtrustningTyp == "Utrustningspaket" &&
                ValtUtrustningPaket != null)
            {
                artikelTypNummer =
                    ValtUtrustningPaket.ArtikelTypNummer;
            }

            if (ValdUtrustningTyp == "Enskild utrustning" &&
                ValdEnskildUtrustning != null)
            {
                artikelTypNummer =
                    ValdEnskildUtrustning.ArtikelTypNummer;
            }

            if (!artikelTypNummer.HasValue)
            {
                return;
            }

            PrisController prisController =
                new PrisController();

            Pris? pris =
                prisController.HämtaAktuelltPris(
                    artikelTypNummer.Value,
                    StartDatum.Value,
                    SlutDatum.Value);

            if (pris == null)
            {
                return;
            }

            if (ValdUtrustningTyp == "Utrustningspaket")
            {
                int antalPaket =
                    AntalUtrustningsPaket ?? 1;

                _aktuelltUtrustningPris =
                    pris.PrisBelopp * antalPaket;
            }
            else
            {
                _aktuelltUtrustningPris =
                    pris.PrisBelopp;
            }

            OnPropertyChanged(nameof(UtrustningPrisText));
        }

        private void LäggTillUtrustning()
        {
            if (!ValideraDatum())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(ValdUtrustningTyp))
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja hur utrustningen ska bokas.");

                return;
            }

            if (ValdUtrustningTyp == "Enskild utrustning")
            {
                LäggTillEnskildUtrustning();
                return;
            }

            if (ValdUtrustningTyp == "Utrustningspaket")
            {
                LäggTillUtrustningsPaket();
                return;
            }
        }

        private void LäggTillEnskildUtrustning()
        {
            if (ValdEnskildUtrustning == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja utrustning.");

                return;
            }

            if (!ValdEnskildUtrustning.ArtikelTypNummer.HasValue)
            {
                VisaMeddelande?.Invoke(
                    "Den valda utrustningen saknar artikeltyp.");

                return;
            }

            PrisController prisController =
                new PrisController();

            Pris? pris =
                prisController.HämtaAktuelltPris(
                    ValdEnskildUtrustning.ArtikelTypNummer.Value,
                    StartDatum!.Value,
                    SlutDatum!.Value);

            if (pris == null)
            {
                VisaMeddelande?.Invoke(
                    "Kunde inte hitta något pris för utrustningen.");

                return;
            }

            UtrustningRad utrustningRad =
                new UtrustningRad
                {
                    StartDatum = StartDatum.Value,
                    SlutDatum = SlutDatum.Value,
                    UtrustningBelopp = pris.PrisBelopp,
                    UtrustningNummer =
                        ValdEnskildUtrustning.UtrustningNummer,
                    UtrustningPaketNummer = null,
                    UtrustningDisplayText =
                        $"Utrustning {ValdEnskildUtrustning.UtrustningNummer}",
                    SenastUppdaterad = DateTime.Now
                };

            UtrustningRader.Add(utrustningRad);

            OnPropertyChanged(nameof(TotalBeloppText));

            ValdEnskildUtrustning = null;
            _aktuelltUtrustningPris = null;

            OnPropertyChanged(nameof(UtrustningPrisText));

            UppdateraTillgängligUtrustning();
        }

        private void LäggTillUtrustningsPaket()
        {
            if (ValtUtrustningPaket == null)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja ett utrustningspaket.");

                return;
            }

            if (AntalUtrustningsPaket == null ||
                AntalUtrustningsPaket <= 0)
            {
                VisaMeddelande?.Invoke(
                    "Du måste välja antal paket.");

                return;
            }

            if (!ValtUtrustningPaket.ArtikelTypNummer.HasValue)
            {
                VisaMeddelande?.Invoke(
                    "Utrustningspaketet saknar artikeltyp.");

                return;
            }

            UtrustningController utrustningController =
                new UtrustningController();

            PrisController prisController =
                new PrisController();

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

            HashSet<string> användaNummer =
                new HashSet<string>();

            for (int paketIndex = 0;
                 paketIndex < AntalUtrustningsPaket.Value;
                 paketIndex++)
            {
                UtrustningRad utrustningRad =
                    new UtrustningRad
                    {
                        StartDatum = StartDatum.Value,
                        SlutDatum = SlutDatum.Value,
                        UtrustningBelopp = pris.PrisBelopp,
                        UtrustningPaketNummer =
                            ValtUtrustningPaket.UtrustningPaketNummer,
                        UtrustningNummer = null,
                        UtrustningDisplayText =
                            ValtUtrustningPaket.UtrustningPaketNamn,
                        SenastUppdaterad = DateTime.Now
                    };

                foreach (UtrustningPaketInnehåll innehållRad in innehåll)
                {
                    if (!innehållRad.ArtikelTypNummer.HasValue)
                    {
                        continue;
                    }

                    var matchandeUtrustningar =
                        ledigaUtrustningar
                            .Where(u =>
                                u.ArtikelTypNummer ==
                                innehållRad.ArtikelTypNummer.Value)

                            .Where(u =>
                                !användaNummer.Contains(
                                    u.UtrustningNummer))

                            .Take(innehållRad.Antal)

                            .ToList();

                    if (matchandeUtrustningar.Count <
                        innehållRad.Antal)
                    {
                        VisaMeddelande?.Invoke(
                            "Det finns inte tillräckligt med ledig fysisk utrustning för paketet.");

                        return;
                    }

                    foreach (var utrustning in matchandeUtrustningar)
                    {
                        utrustningRad.UtrustningPaketRader.Add(
                            new UtrustningPaketRad
                            {
                                UtrustningNummer =
                                    utrustning.UtrustningNummer
                            });

                        användaNummer.Add(
                            utrustning.UtrustningNummer);
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

        #region PropertyChanged
        public event PropertyChangedEventHandler?
            PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

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
                AntalUtrustningsPaket = null;
            }
        }

        public bool VisarUtrustningsPaket =>
            ValdUtrustningTyp == "Utrustningspaket";

        public bool VisarEnskildUtrustning =>
            ValdUtrustningTyp == "Enskild utrustning";


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


        public string UtrustningPrisText =>
            _aktuelltUtrustningPris.HasValue
                ? $"{_aktuelltUtrustningPris.Value:N0} kr"
                : "0 kr";

        #region RelayCommands

        private sealed class RelayCommand : ICommand
        {
            private readonly Action<object?>
                _execute;

            private readonly Predicate<object?>?
                _canExecute;

                        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
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
                CanExecuteChanged?.Invoke( this, EventArgs.Empty);
            }

        }
        #endregion
    }
}