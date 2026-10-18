using Datalager;
using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicelager
{
    public class StatistikController
    {
        // Ett ogiltigt datumintervall ska inte accepteras
        public bool ÄrGiltigtIntervall(DateTime? startDatum, DateTime? slutDatum, out string felmeddelande)
        {
            if (startDatum == null || slutDatum == null)
            {
                felmeddelande = "Både startdatum och slutdatum måste anges.";
                return false;
            }

            if (startDatum.Value.Date > slutDatum.Value.Date)
            {
                felmeddelande = "Startdatum kan inte vara senare än slutdatum.";
                return false;
            }

            felmeddelande = string.Empty;
            return true;
        }

        // Endast information inom valt datumintervall ska visas.
        // Vistelsedatumen styr, och bara den del som ligger inom intervallet räknas.
        public StatistikResultat HämtaStatistik(DateTime startDatum, DateTime slutDatum)
        {
            if (!ÄrGiltigtIntervall(startDatum, slutDatum, out string felmeddelande))
            {
                throw new ArgumentException(felmeddelande);
            }

            DateTime intervallStart = startDatum.Date;
            DateTime intervallSlut = slutDatum.Date.AddDays(1); // hela slutdagen räknas med
            int antalDagarIIntervall = (intervallSlut - intervallStart).Days;

            using var unitOfWork = new UnitOfWork(new SkiContext());

            var logiRader = unitOfWork.LogiRadRepository
                .Find(r => r.StartDatum < intervallSlut && r.SlutDatum > intervallStart)
                .ToList();

            var utrustningRader = unitOfWork.UtrustningRadRepository
                .Find(r => r.StartDatum < intervallSlut && r.SlutDatum >= intervallStart)
                .ToList();

            var skidlektionRader = unitOfWork.SkidlektionRadRepository
                .Find(r => r.Datum >= intervallStart && r.Datum < intervallSlut)
                .ToList();

            var konferensRader = unitOfWork.KonferensRadRepository
                .Find(r => r.StartTid < intervallSlut && r.SlutTid >= intervallStart)
                .ToList();

            var lektioner = unitOfWork.SkidlektionRepository
                .Find(l => l.Datum >= intervallStart && l.Datum < intervallSlut)
                .ToList();

            var allaLogi = unitOfWork.LogiRepository.GetAll().ToList();
            int antalUtrustning = unitOfWork.UtrustningRepository.GetAll().Count();

            var resultat = new StatistikResultat
            {
                StartDatum = intervallStart,
                SlutDatum = slutDatum.Date
            };

            // Förbered en rad per logialternativ (grupperat på artikeltyp, t.ex. "Stuga")
            var perAlternativ = new Dictionary<string, LogiStatistikRad>();
            var typPerLogi = new Dictionary<string, string>();

            foreach (var logi in allaLogi)
            {
                string typ = logi.artikelTyp?.TypNamn ?? "Övrigt";
                typPerLogi[logi.LogiNummer] = typ;

                if (!perAlternativ.ContainsKey(typ))
                {
                    perAlternativ[typ] = new LogiStatistikRad { Logialternativ = typ };
                }
                perAlternativ[typ].AntalEnheter++;
            }

            // Logi: räknas i nätter
            foreach (var rad in logiRader)
            {
                DateTime start = rad.StartDatum.Date;
                DateTime slut = rad.SlutDatum.Date;
                int totalaNätter = (slut - start).Days;
                int nätterInom = Överlapp(start, slut, intervallStart, intervallSlut);
                decimal intäkt = Andel(rad.LogiBelopp, nätterInom, totalaNätter);

                resultat.BokadeLogiNätter += nätterInom;
                resultat.IntäktLogi += intäkt;

                // Lägg även till på rätt logialternativ
                if (rad.LogiNummer != null && typPerLogi.ContainsKey(rad.LogiNummer))
                {
                    var alternativ = perAlternativ[typPerLogi[rad.LogiNummer]];
                    alternativ.BokadeNätter += nätterInom;
                    alternativ.Intäkt += intäkt;
                }
            }

            // Utrustning: räknas i dagar, sista dagen räknas med
            foreach (var rad in utrustningRader)
            {
                DateTime start = rad.StartDatum.Date;
                DateTime slut = rad.SlutDatum.Date.AddDays(1);
                int totalaDagar = (slut - start).Days;
                int dagarInom = Överlapp(start, slut, intervallStart, intervallSlut);

                resultat.IntäktUtrustning += Andel(rad.UtrustningBelopp, dagarInom, totalaDagar);

                if (rad.UtrustningNummer != null)
                {
                    resultat.BokadeUtrustningDagar += dagarInom;
                }
            }

            // Skidlektioner: intäkt och bokade platser
            foreach (var rad in skidlektionRader)
            {
                resultat.IntäktSkidlektion += rad.SkidlektionBelopp;
                resultat.BokadeLektionsPlatser += rad.AntalPersoner;
            }

            // Konferens: räknas i dagar, sista dagen räknas med
            foreach (var rad in konferensRader)
            {
                DateTime start = rad.StartTid.Date;
                DateTime slut = rad.SlutTid.Date.AddDays(1);
                int totalaDagar = (slut - start).Days;
                int dagarInom = Överlapp(start, slut, intervallStart, intervallSlut);

                resultat.IntäktKonferens += Andel(rad.KonferensBelopp, dagarInom, totalaDagar);
            }

            // Antal unika bokningar
            resultat.AntalBokningar = logiRader.Select(r => r.BokningsNummer)
                .Concat(utrustningRader.Select(r => r.BokningsNummer))
                .Concat(skidlektionRader.Select(r => r.BokningsNummer))
                .Concat(konferensRader.Select(r => r.BokningsNummer))
                .Where(nummer => nummer != null)
                .Distinct()
                .Count();

            // Beläggning logi (totalt)
            resultat.TillgängligaLogiNätter = allaLogi.Count * antalDagarIIntervall;
            resultat.BeläggningLogiProcent = Procent(resultat.BokadeLogiNätter, resultat.TillgängligaLogiNätter);

            // Beläggning logi per logialternativ
            foreach (var alternativ in perAlternativ.Values)
            {
                alternativ.TillgängligaNätter = alternativ.AntalEnheter * antalDagarIIntervall;
                alternativ.BeläggningProcent = Procent(alternativ.BokadeNätter, alternativ.TillgängligaNätter);
                alternativ.Intäkt = Math.Round(alternativ.Intäkt, 2);
            }
            resultat.LogiPerAlternativ = perAlternativ.Values.OrderBy(a => a.Logialternativ).ToList();

            // Beläggning utrustning
            resultat.TillgängligaUtrustningDagar = antalUtrustning * antalDagarIIntervall;
            resultat.BeläggningUtrustningProcent = Procent(resultat.BokadeUtrustningDagar, resultat.TillgängligaUtrustningDagar);

            // Beläggning skidlektioner: bokade platser / kapacitet för lektioner inom intervallet
            resultat.AntalLektioner = lektioner.Count;
            resultat.TillgängligaLektionsPlatser = lektioner.Sum(l => l.Kapacitet);
            resultat.BeläggningSkidlektionProcent = Procent(resultat.BokadeLektionsPlatser, resultat.TillgängligaLektionsPlatser);

            resultat.IntäktLogi = Math.Round(resultat.IntäktLogi, 2);
            resultat.IntäktUtrustning = Math.Round(resultat.IntäktUtrustning, 2);
            resultat.IntäktSkidlektion = Math.Round(resultat.IntäktSkidlektion, 2);
            resultat.IntäktKonferens = Math.Round(resultat.IntäktKonferens, 2);

            return resultat;
        }

        #region Hjälpmetoder

        // Antal dagar som en period överlappar intervallet
        private int Överlapp(DateTime start, DateTime slut, DateTime intervallStart, DateTime intervallSlut)
        {
            DateTime senasteStart = start > intervallStart ? start : intervallStart;
            DateTime tidigasteSlut = slut < intervallSlut ? slut : intervallSlut;
            int dagar = (tidigasteSlut - senasteStart).Days;
            return dagar > 0 ? dagar : 0;
        }

        // Den del av beloppet som hör till dagarna inom intervallet
        private decimal Andel(decimal belopp, int dagarInom, int totalaDagar)
        {
            if (totalaDagar <= 0)
            {
                return 0;
            }
            return belopp * dagarInom / totalaDagar;
        }

        private double Procent(int del, int helhet)
        {
            if (helhet <= 0)
            {
                return 0;
            }
            return Math.Round(100.0 * del / helhet, 1);
        }

        #endregion
    }

    // Samlar all statistik så att den kan skickas till vyn i ett paket
    public class StatistikResultat
    {
        public DateTime StartDatum { get; set; }
        public DateTime SlutDatum { get; set; }

        public int AntalBokningar { get; set; }

        // Intäkter
        public decimal IntäktLogi { get; set; }
        public decimal IntäktUtrustning { get; set; }
        public decimal IntäktSkidlektion { get; set; }
        public decimal IntäktKonferens { get; set; }
        public decimal TotalIntäkt => IntäktLogi + IntäktUtrustning + IntäktSkidlektion + IntäktKonferens;

        // Beläggning logi (totalt)
        public int BokadeLogiNätter { get; set; }
        public int TillgängligaLogiNätter { get; set; }
        public double BeläggningLogiProcent { get; set; }

        // Statistik per logialternativ
        public List<LogiStatistikRad> LogiPerAlternativ { get; set; } = new List<LogiStatistikRad>();

        // Beläggning utrustning
        public int BokadeUtrustningDagar { get; set; }
        public int TillgängligaUtrustningDagar { get; set; }
        public double BeläggningUtrustningProcent { get; set; }

        // Beläggning skidlektioner
        public int AntalLektioner { get; set; }
        public int BokadeLektionsPlatser { get; set; }
        public int TillgängligaLektionsPlatser { get; set; }
        public double BeläggningSkidlektionProcent { get; set; }
    }

    // En rad i tabellen "Logi per logialternativ"
    public class LogiStatistikRad
    {
        public string Logialternativ { get; set; } = string.Empty;
        public int AntalEnheter { get; set; }
        public int BokadeNätter { get; set; }
        public int TillgängligaNätter { get; set; }
        public double BeläggningProcent { get; set; }
        public decimal Intäkt { get; set; }
    }
}