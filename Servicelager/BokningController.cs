using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class BokningController
    {

        public void SkapaBokning(Bokning bokning)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            unitOfWork.BokningRepository.Add(bokning);

            unitOfWork.Save();
        }


        public List<Bokning> HämtaAllaBokningar()
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            List<Bokning> bokningar =
                unitOfWork.BokningRepository
                    .GetAll()
                    .OrderByDescending(b => b.BokningsNummer)
                    .ToList();

            List<Kund> kunder =
                unitOfWork.KundRepository
                    .GetAll()
                    .ToList();

            foreach (Bokning bokning in bokningar)
            {
                bokning.kund = kunder.FirstOrDefault(k => k.KundNummer == bokning.KundNummer);
            }

            return bokningar;
        }


        public List<Bokning> SökBokningar(string? söktext)
        {
            List<Bokning> bokningar = HämtaAllaBokningar();

            if (string.IsNullOrWhiteSpace(söktext))
                return bokningar;

            string sökning = söktext.Trim();

            return bokningar
                .Where(b =>
                    b.BokningsNummer
                        .ToString()
                        .Contains(sökning,
                            StringComparison.OrdinalIgnoreCase)

                    ||

                    (b.KundNummer.HasValue &&
                     b.KundNummer.Value
                         .ToString()
                         .Contains(sökning,
                             StringComparison.OrdinalIgnoreCase))

                    ||

                    (b.kund != null &&
                     b.kund.VisningsNamn
                         .Contains(sökning,
                             StringComparison.OrdinalIgnoreCase))
                )
                .OrderByDescending(b => b.BokningsNummer)
                .ToList();
        }

        public Bokning? HämtaBokningMedRader(int bokningsNummer)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            Bokning? bokning = unitOfWork.BokningRepository
                .GetAll()
                .FirstOrDefault(b => b.BokningsNummer == bokningsNummer);

            if (bokning == null)
                return null;


            // =========================================
            // KUND
            // =========================================

            Kund? kund = unitOfWork.KundRepository
                .GetAll()
                .FirstOrDefault(k => k.KundNummer == bokning.KundNummer);


            // =========================================
            // HJÄLPDATA
            // =========================================

            List<ArtikelTyp> artikelTyper = unitOfWork.ArtikelTypRepository
                .GetAll()
                .ToList();

            List<Logi> allaLogi = unitOfWork.LogiRepository
                .GetAll()
                .ToList();

            List<Utrustning> allUtrustning = unitOfWork.UtrustningRepository
                .GetAll()
                .ToList();

            List<UtrustningPaket> allaUtrustningsPaket =
                unitOfWork.UtrustningPaketRepository
                    .GetAll()
                    .ToList();

            List<UtrustningPaketRad> allaUtrustningPaketRader =
                unitOfWork.UtrustningPaketRadRepository
                    .GetAll()
                    .ToList();

            List<Konferens> allaKonferenser =
                unitOfWork.KonferensRepository
                    .GetAll()
                    .ToList();

            List<Skidlektion> allaSkidlektioner =
                unitOfWork.SkidlektionRepository
                    .GetAll()
                    .ToList();


            // =========================================
            // LOGI
            // =========================================

            List<LogiRad> logiRaderFrånDatabas =
                unitOfWork.LogiRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<LogiRad> logiRader = new List<LogiRad>();

            foreach (LogiRad dbRad in logiRaderFrånDatabas)
            {
                Logi? dbLogi = allaLogi
                    .FirstOrDefault(l => l.LogiNummer == dbRad.LogiNummer);

                Logi? vanligtLogi = null;

                if (dbLogi != null)
                {
                    ArtikelTyp? dbArtikelTyp = artikelTyper
                        .FirstOrDefault(a =>
                            a.ArtikelTypNummer == dbLogi.ArtikelTypNummer);

                    vanligtLogi = new Logi
                    {
                        LogiNummer = dbLogi.LogiNummer,
                        LogiKapacitet = dbLogi.LogiKapacitet,
                        Status = dbLogi.Status,
                        SenastUppdaterad = dbLogi.SenastUppdaterad,
                        AntalRum = dbLogi.AntalRum,
                        Storlek = dbLogi.Storlek,
                        Faciliteter = dbLogi.Faciliteter,
                        ArtikelTypNummer = dbLogi.ArtikelTypNummer,

                        artikelTyp = dbArtikelTyp
                    };
                }

                LogiRad nyRad = new LogiRad
                {
                    LogiRadNummer = dbRad.LogiRadNummer,
                    StartDatum = dbRad.StartDatum,
                    SlutDatum = dbRad.SlutDatum,
                    LogiBelopp = dbRad.LogiBelopp,
                    AntalPersoner = dbRad.AntalPersoner,
                    SenastUppdaterad = dbRad.SenastUppdaterad,
                    LogiNummer = dbRad.LogiNummer,
                    BokningsNummer = dbRad.BokningsNummer,

                    logi = vanligtLogi,

                    LogiDisplayText =
                        vanligtLogi?.DisplayText
                        ?? dbRad.LogiNummer
                        ?? "Boende"
                };

                logiRader.Add(nyRad);
            }


            // =========================================
            // UTRUSTNING
            // =========================================

            List<UtrustningRad> utrustningRaderFrånDatabas =
                unitOfWork.UtrustningRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<UtrustningRad> utrustningRader =
                new List<UtrustningRad>();

            foreach (UtrustningRad dbRad in utrustningRaderFrånDatabas)
            {
                UtrustningRad nyRad = new UtrustningRad
                {
                    UtrustningRadNummer = dbRad.UtrustningRadNummer,
                    StartDatum = dbRad.StartDatum,
                    SlutDatum = dbRad.SlutDatum,
                    UtrustningBelopp = dbRad.UtrustningBelopp,
                    SenastUppdaterad = dbRad.SenastUppdaterad,

                    UtrustningPaketNummer =
                        dbRad.UtrustningPaketNummer,

                    UtrustningNummer =
                        dbRad.UtrustningNummer,

                    BokningsNummer =
                        dbRad.BokningsNummer,

                    UtrustningPaketRader =
                        new List<UtrustningPaketRad>()
                };


                // -----------------------------------------
                // UTRUSTNINGSPAKET
                // -----------------------------------------

                if (dbRad.UtrustningPaketNummer.HasValue)
                {
                    UtrustningPaket? dbPaket =
                        allaUtrustningsPaket
                            .FirstOrDefault(p =>
                                p.UtrustningPaketNummer ==
                                dbRad.UtrustningPaketNummer.Value);

                    if (dbPaket != null)
                    {
                        nyRad.utrustningPaket =
                            new UtrustningPaket
                            {
                                UtrustningPaketNummer =
                                    dbPaket.UtrustningPaketNummer,

                                UtrustningPaketNamn =
                                    dbPaket.UtrustningPaketNamn,

                                ArtikelTypNummer =
                                    dbPaket.ArtikelTypNummer,

                                artikelTyp = artikelTyper
                                    .FirstOrDefault(a =>
                                        a.ArtikelTypNummer ==
                                        dbPaket.ArtikelTypNummer)
                            };

                        nyRad.UtrustningDisplayText =
                            dbPaket.UtrustningPaketNamn;
                    }


                    // Fysiska utrustningsobjekt som hör till paket-raden

                    List<UtrustningPaketRad> paketRader =
                        allaUtrustningPaketRader
                            .Where(pr =>
                                pr.UtrustningRadNummer ==
                                dbRad.UtrustningRadNummer)
                            .ToList();

                    foreach (UtrustningPaketRad dbPaketRad in paketRader)
                    {
                        Utrustning? dbUtrustning =
                            allUtrustning
                                .FirstOrDefault(u =>
                                    u.UtrustningNummer ==
                                    dbPaketRad.UtrustningNummer);

                        Utrustning? vanligUtrustning = null;

                        if (dbUtrustning != null)
                        {
                            vanligUtrustning =
                                new Utrustning
                                {
                                    UtrustningNummer =
                                        dbUtrustning.UtrustningNummer,

                                    Status =
                                        dbUtrustning.Status,

                                    SenastUppdaterad =
                                        dbUtrustning.SenastUppdaterad,

                                    ArtikelTypNummer =
                                        dbUtrustning.ArtikelTypNummer,

                                    artikelTyp = artikelTyper
                                        .FirstOrDefault(a =>
                                            a.ArtikelTypNummer ==
                                            dbUtrustning.ArtikelTypNummer)
                                };
                        }

                        UtrustningPaketRad nyPaketRad =
                            new UtrustningPaketRad
                            {
                                UtrustningPaketRadNummer =
                                    dbPaketRad.UtrustningPaketRadNummer,

                                UtrustningNummer =
                                    dbPaketRad.UtrustningNummer,

                                UtrustningRadNummer =
                                    dbPaketRad.UtrustningRadNummer,

                                utrustning =
                                    vanligUtrustning
                            };

                        nyRad.UtrustningPaketRader.Add(
                            nyPaketRad);
                    }
                }


                // -----------------------------------------
                // ENSKILD UTRUSTNING
                // -----------------------------------------

                else if (dbRad.UtrustningNummer != null)
                {
                    Utrustning? dbUtrustning =
                        allUtrustning
                            .FirstOrDefault(u =>
                                u.UtrustningNummer ==
                                dbRad.UtrustningNummer);

                    if (dbUtrustning != null)
                    {
                        ArtikelTyp? artikelTyp =
                            artikelTyper
                                .FirstOrDefault(a =>
                                    a.ArtikelTypNummer ==
                                    dbUtrustning.ArtikelTypNummer);

                        nyRad.utrustning =
                            new Utrustning
                            {
                                UtrustningNummer =
                                    dbUtrustning.UtrustningNummer,

                                Status =
                                    dbUtrustning.Status,

                                SenastUppdaterad =
                                    dbUtrustning.SenastUppdaterad,

                                ArtikelTypNummer =
                                    dbUtrustning.ArtikelTypNummer,

                                artikelTyp =
                                    artikelTyp
                            };

                        nyRad.UtrustningDisplayText =
                            artikelTyp?.TypNamn
                            ?? $"Utrustning {dbUtrustning.UtrustningNummer}";
                    }
                }

                utrustningRader.Add(nyRad);
            }


            // =========================================
            // KONFERENS
            // =========================================

            List<KonferensRad> konferensRaderFrånDatabas =
                unitOfWork.KonferensRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<KonferensRad> konferensRader =
                new List<KonferensRad>();

            foreach (KonferensRad dbRad in konferensRaderFrånDatabas)
            {
                Konferens? dbKonferens =
                    allaKonferenser
                        .FirstOrDefault(k =>
                            k.KonferensNummer ==
                            dbRad.KonferensNummer);

                Konferens? vanligKonferens = null;

                if (dbKonferens != null)
                {
                    vanligKonferens =
                        new Konferens
                        {
                            KonferensNummer =
                                dbKonferens.KonferensNummer,

                            KonferensKapacitet =
                                dbKonferens.KonferensKapacitet,

                            Status =
                                dbKonferens.Status,

                            SenastUppdaterad =
                                dbKonferens.SenastUppdaterad,

                            ArtikelTypNummer =
                                dbKonferens.ArtikelTypNummer,

                            artikelTyp = artikelTyper
                                .FirstOrDefault(a =>
                                    a.ArtikelTypNummer ==
                                    dbKonferens.ArtikelTypNummer)
                        };
                }

                KonferensRad nyRad =
                    new KonferensRad
                    {
                        KonferensRadNummer =
                            dbRad.KonferensRadNummer,

                        StartTid =
                            dbRad.StartTid,

                        SlutTid =
                            dbRad.SlutTid,

                        KonferensBelopp =
                            dbRad.KonferensBelopp,

                        AntalPersoner =
                            dbRad.AntalPersoner,

                        SenastUppdaterad =
                            dbRad.SenastUppdaterad,

                        KonferensNummer =
                            dbRad.KonferensNummer,

                        BokningsNummer =
                            dbRad.BokningsNummer,

                        konferens =
                            vanligKonferens
                    };

                konferensRader.Add(nyRad);
            }


            // =========================================
            // SKIDLEKTION
            // =========================================

            List<SkidlektionRad> skidlektionRaderFrånDatabas =
                unitOfWork.SkidlektionRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<SkidlektionRad> skidlektionRader =
                new List<SkidlektionRad>();

            foreach (SkidlektionRad dbRad in skidlektionRaderFrånDatabas)
            {
                Skidlektion? dbSkidlektion =
                    allaSkidlektioner
                        .FirstOrDefault(s =>
                            s.SkidlektionNummer ==
                            dbRad.SkidlektionNummer);

                Skidlektion? vanligSkidlektion = null;

                string displayText = "Skidlektion";

                if (dbSkidlektion != null)
                {
                    ArtikelTyp? artikelTyp =
                        artikelTyper
                            .FirstOrDefault(a =>
                                a.ArtikelTypNummer ==
                                dbSkidlektion.ArtikelTypNummer);

                    vanligSkidlektion =
                        new Skidlektion
                        {
                            SkidlektionNummer =
                                dbSkidlektion.SkidlektionNummer,

                            Datum =
                                dbSkidlektion.Datum,

                            StartTid =
                                dbSkidlektion.StartTid,

                            SlutTid =
                                dbSkidlektion.SlutTid,

                            Status =
                                dbSkidlektion.Status,

                            Kapacitet =
                                dbSkidlektion.Kapacitet,

                            SenastUppdaterad =
                                dbSkidlektion.SenastUppdaterad,

                            AnställningsNummer =
                                dbSkidlektion.AnställningsNummer,

                            ArtikelTypNummer =
                                dbSkidlektion.ArtikelTypNummer,

                            artikelTyp =
                                artikelTyp
                        };

                    displayText =
                        $"{artikelTyp?.TypNamn ?? "Skidlektion"} | " +
                        $"{dbSkidlektion.StartTid:hh\\:mm} - " +
                        $"{dbSkidlektion.SlutTid:hh\\:mm}";
                }

                SkidlektionRad nyRad =
                    new SkidlektionRad
                    {
                        SkidLektionRadNummer =
                            dbRad.SkidLektionRadNummer,

                        Datum =
                            dbRad.Datum,

                        SkidlektionBelopp =
                            dbRad.SkidlektionBelopp,

                        AntalPersoner =
                            dbRad.AntalPersoner,

                        SenastUppdaterad =
                            dbRad.SenastUppdaterad,

                        SkidlektionNummer =
                            dbRad.SkidlektionNummer,

                        BokningsNummer =
                            dbRad.BokningsNummer,

                        skidlektion =
                            vanligSkidlektion,

                        SkidlektionDisplayText =
                            displayText
                    };

                skidlektionRader.Add(nyRad);
            }


            // =========================================
            // FRISTÅENDE BOKNING
            // =========================================

            Bokning resultat =
                new Bokning
                {
                    BokningsNummer =
                        bokning.BokningsNummer,

                    BokningsDatum =
                        bokning.BokningsDatum,

                    Status =
                        bokning.Status,

                    KundNummer =
                        bokning.KundNummer,

                    kund =
                        kund,

                    LogiRader =
                        logiRader,

                    UtrustningRader =
                        utrustningRader,

                    KonferensRader =
                        konferensRader,

                    SkidlektionRader =
                        skidlektionRader
                };

            return resultat;
        }

        public void SparaÄndringar(int bokningsNummer, int? kundNummer, List<LogiRad> logiRader, List<UtrustningRad> utrustningRader, List<KonferensRad> konferensRader, List<SkidlektionRad> skidlektionRader)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());


            // =========================================
            // BOKNING
            // =========================================

            Bokning? bokning = unitOfWork.BokningRepository
                .GetAll()
                .FirstOrDefault(b => b.BokningsNummer == bokningsNummer);

            if (bokning == null)
                throw new InvalidOperationException("Bokningen kunde inte hittas.");

            bokning.KundNummer = kundNummer;

            unitOfWork.BokningRepository.Update(bokning);


            // =========================================
            // LOGI
            // =========================================

            List<LogiRad> befintligaLogiRader =
                unitOfWork.LogiRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (LogiRad befintligRad in befintligaLogiRader)
            {
                bool finnsKvar = logiRader.Any(r =>
                    r.LogiRadNummer == befintligRad.LogiRadNummer);

                if (!finnsKvar)
                {
                    unitOfWork.LogiRadRepository.Remove(befintligRad);
                }
            }

            foreach (LogiRad rad in logiRader)
            {
                if (rad.LogiRadNummer == 0)
                {
                    LogiRad nyRad = new LogiRad
                    {
                        StartDatum = rad.StartDatum,
                        SlutDatum = rad.SlutDatum,
                        LogiBelopp = rad.LogiBelopp,
                        AntalPersoner = rad.AntalPersoner,
                        SenastUppdaterad = DateTime.Now,
                        LogiNummer = rad.LogiNummer,
                        BokningsNummer = bokningsNummer
                    };

                    unitOfWork.LogiRadRepository.Add(nyRad);
                }
                else
                {
                    LogiRad? befintligRad = befintligaLogiRader
                        .FirstOrDefault(r =>
                            r.LogiRadNummer == rad.LogiRadNummer);

                    if (befintligRad != null)
                    {
                        befintligRad.StartDatum = rad.StartDatum;
                        befintligRad.SlutDatum = rad.SlutDatum;
                        befintligRad.LogiBelopp = rad.LogiBelopp;
                        befintligRad.AntalPersoner = rad.AntalPersoner;
                        befintligRad.LogiNummer = rad.LogiNummer;
                        befintligRad.SenastUppdaterad = DateTime.Now;

                        unitOfWork.LogiRadRepository.Update(befintligRad);
                    }
                }
            }


            // =========================================
            // UTRUSTNING
            // =========================================

            List<UtrustningRad> befintligaUtrustningRader =
                unitOfWork.UtrustningRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<UtrustningPaketRad> befintligaPaketRader =
                unitOfWork.UtrustningPaketRadRepository
                    .GetAll()
                    .ToList();


            // -----------------------------------------
            // TA BORT UTRUSTNINGSRADER SOM INTE FINNS KVAR
            // -----------------------------------------

            foreach (UtrustningRad befintligRad in befintligaUtrustningRader)
            {
                bool finnsKvar = utrustningRader.Any(r =>
                    r.UtrustningRadNummer ==
                    befintligRad.UtrustningRadNummer);

                if (!finnsKvar)
                {
                    // Ta först bort paket-rader som pekar på utrustningsraden
                    List<UtrustningPaketRad> paketRaderAttTaBort =
                        befintligaPaketRader
                            .Where(pr =>
                                pr.UtrustningRadNummer ==
                                befintligRad.UtrustningRadNummer)
                            .ToList();

                    foreach (UtrustningPaketRad paketRad in paketRaderAttTaBort)
                    {
                        unitOfWork.UtrustningPaketRadRepository
                            .Remove(paketRad);
                    }

                    // Därefter kan själva utrustningsraden tas bort
                    unitOfWork.UtrustningRadRepository
                        .Remove(befintligRad);
                }
            }


            // -----------------------------------------
            // LÄGG TILL / UPPDATERA UTRUSTNING
            // -----------------------------------------

            foreach (UtrustningRad rad in utrustningRader)
            {
                // NY RAD
                if (rad.UtrustningRadNummer == 0)
                {
                    UtrustningRad nyRad = new UtrustningRad
                    {
                        StartDatum = rad.StartDatum,
                        SlutDatum = rad.SlutDatum,
                        UtrustningBelopp = rad.UtrustningBelopp,
                        SenastUppdaterad = DateTime.Now,

                        UtrustningPaketNummer =
                            rad.UtrustningPaketNummer,

                        UtrustningNummer =
                            rad.UtrustningNummer,

                        BokningsNummer =
                            bokningsNummer
                    };

                    unitOfWork.UtrustningRadRepository.Add(nyRad);

                    // Spara först så att nyRad får sitt UtrustningRadNummer
                    unitOfWork.Save();


                    // Om den nya raden är ett paket måste paketets fysiska
                    // utrustningsrader också sparas
                    if (rad.UtrustningPaketRader != null &&
                        rad.UtrustningPaketRader.Any())
                    {
                        foreach (UtrustningPaketRad paketRad in rad.UtrustningPaketRader)
                        {
                            UtrustningPaketRad nyPaketRad =
                                new UtrustningPaketRad
                                {
                                    UtrustningNummer =
                                        paketRad.UtrustningNummer,

                                    UtrustningRadNummer =
                                        nyRad.UtrustningRadNummer
                                };

                            unitOfWork.UtrustningPaketRadRepository
                                .Add(nyPaketRad);
                        }
                    }
                }

                // BEFINTLIG RAD
                else
                {
                    UtrustningRad? befintligRad =
                        befintligaUtrustningRader
                            .FirstOrDefault(r =>
                                r.UtrustningRadNummer ==
                                rad.UtrustningRadNummer);

                    if (befintligRad != null)
                    {
                        befintligRad.StartDatum =
                            rad.StartDatum;

                        befintligRad.SlutDatum =
                            rad.SlutDatum;

                        befintligRad.UtrustningBelopp =
                            rad.UtrustningBelopp;

                        befintligRad.UtrustningPaketNummer =
                            rad.UtrustningPaketNummer;

                        befintligRad.UtrustningNummer =
                            rad.UtrustningNummer;

                        befintligRad.SenastUppdaterad =
                            DateTime.Now;

                        unitOfWork.UtrustningRadRepository
                            .Update(befintligRad);
                    }
                }
            }


            // =========================================
            // KONFERENS
            // =========================================

            List<KonferensRad> befintligaKonferensRader =
                unitOfWork.KonferensRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (KonferensRad befintligRad in befintligaKonferensRader)
            {
                bool finnsKvar = konferensRader.Any(r =>
                    r.KonferensRadNummer ==
                    befintligRad.KonferensRadNummer);

                if (!finnsKvar)
                {
                    unitOfWork.KonferensRadRepository
                        .Remove(befintligRad);
                }
            }

            foreach (KonferensRad rad in konferensRader)
            {
                if (rad.KonferensRadNummer == 0)
                {
                    KonferensRad nyRad = new KonferensRad
                    {
                        StartTid = rad.StartTid,
                        SlutTid = rad.SlutTid,
                        KonferensBelopp = rad.KonferensBelopp,
                        AntalPersoner = rad.AntalPersoner,
                        KonferensNummer = rad.KonferensNummer,
                        BokningsNummer = bokningsNummer,
                        SenastUppdaterad = DateTime.Now
                    };

                    unitOfWork.KonferensRadRepository.Add(nyRad);
                }
                else
                {
                    KonferensRad? befintligRad =
                        befintligaKonferensRader
                            .FirstOrDefault(r =>
                                r.KonferensRadNummer ==
                                rad.KonferensRadNummer);

                    if (befintligRad != null)
                    {
                        befintligRad.StartTid = rad.StartTid;
                        befintligRad.SlutTid = rad.SlutTid;
                        befintligRad.KonferensBelopp =
                            rad.KonferensBelopp;

                        befintligRad.AntalPersoner =
                            rad.AntalPersoner;

                        befintligRad.KonferensNummer =
                            rad.KonferensNummer;

                        befintligRad.SenastUppdaterad =
                            DateTime.Now;

                        unitOfWork.KonferensRadRepository
                            .Update(befintligRad);
                    }
                }
            }


            // =========================================
            // SKIDLEKTION
            // =========================================

            List<SkidlektionRad> befintligaSkidlektionRader =
                unitOfWork.SkidlektionRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (SkidlektionRad befintligRad in befintligaSkidlektionRader)
            {
                bool finnsKvar = skidlektionRader.Any(r =>
                    r.SkidLektionRadNummer ==
                    befintligRad.SkidLektionRadNummer);

                if (!finnsKvar)
                {
                    unitOfWork.SkidlektionRadRepository
                        .Remove(befintligRad);
                }
            }

            foreach (SkidlektionRad rad in skidlektionRader)
            {
                if (rad.SkidLektionRadNummer == 0)
                {
                    SkidlektionRad nyRad = new SkidlektionRad
                    {
                        Datum = rad.Datum,
                        SkidlektionBelopp =
                            rad.SkidlektionBelopp,

                        AntalPersoner = rad.AntalPersoner,
                        SkidlektionNummer =
                            rad.SkidlektionNummer,

                        BokningsNummer = bokningsNummer,
                        SenastUppdaterad = DateTime.Now
                    };

                    unitOfWork.SkidlektionRadRepository.Add(nyRad);
                }
                else
                {
                    SkidlektionRad? befintligRad =
                        befintligaSkidlektionRader
                            .FirstOrDefault(r =>
                                r.SkidLektionRadNummer ==
                                rad.SkidLektionRadNummer);

                    if (befintligRad != null)
                    {
                        befintligRad.Datum = rad.Datum;

                        befintligRad.SkidlektionBelopp =
                            rad.SkidlektionBelopp;

                        befintligRad.AntalPersoner =
                            rad.AntalPersoner;

                        befintligRad.SkidlektionNummer =
                            rad.SkidlektionNummer;

                        befintligRad.SenastUppdaterad =
                            DateTime.Now;

                        unitOfWork.SkidlektionRadRepository
                            .Update(befintligRad);
                    }
                }
            }


            // =========================================
            // SPARA ALLT
            // =========================================

            unitOfWork.Save();
        }

        public void TaBortBokning(int bokningsNummer)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            Bokning? bokning = unitOfWork.BokningRepository
                .GetAll()
                .FirstOrDefault(b => b.BokningsNummer == bokningsNummer);

            if (bokning == null)
                throw new InvalidOperationException("Bokningen kunde inte hittas.");


            // UTRUSTNING

            List<UtrustningRad> utrustningRader =
                unitOfWork.UtrustningRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            List<UtrustningPaketRad> paketRader =
                unitOfWork.UtrustningPaketRadRepository
                    .GetAll()
                    .ToList();

            foreach (UtrustningRad utrustningRad in utrustningRader)
            {
                List<UtrustningPaketRad> paketRaderAttTaBort =
                    paketRader
                        .Where(r =>
                            r.UtrustningRadNummer ==
                            utrustningRad.UtrustningRadNummer)
                        .ToList();

                foreach (UtrustningPaketRad paketRad in paketRaderAttTaBort)
                {
                    unitOfWork.UtrustningPaketRadRepository
                        .Remove(paketRad);
                }

                unitOfWork.UtrustningRadRepository
                    .Remove(utrustningRad);
            }


            // BOENDE

            List<LogiRad> logiRader =
                unitOfWork.LogiRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (LogiRad rad in logiRader)
            {
                unitOfWork.LogiRadRepository.Remove(rad);
            }


            // KONFERENS

            List<KonferensRad> konferensRader =
                unitOfWork.KonferensRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (KonferensRad rad in konferensRader)
            {
                unitOfWork.KonferensRadRepository.Remove(rad);
            }


            // SKIDLEKTION

            List<SkidlektionRad> skidlektionRader =
                unitOfWork.SkidlektionRadRepository
                    .GetAll()
                    .Where(r => r.BokningsNummer == bokningsNummer)
                    .ToList();

            foreach (SkidlektionRad rad in skidlektionRader)
            {
                unitOfWork.SkidlektionRadRepository.Remove(rad);
            }


            // BOKNING

            unitOfWork.BokningRepository.Remove(bokning);

            unitOfWork.Save();
        }

    }
}