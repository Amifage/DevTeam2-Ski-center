using Datalager;
using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicelager
{
    public class StatistikController
    {
        public bool ArGiltigtIntervall(DateTime? startDatum, DateTime? slutDatum, out string felmeddelande)
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

        public StatistikResultat HamtaStatistik(DateTime startDatum, DateTime slutDatum)
        {
            if (!ArGiltigtIntervall(startDatum, slutDatum, out string felmeddelande))
            {
                throw new ArgumentException(felmeddelande);
            }

            DateTime intervallStart = startDatum.Date;
            DateTime intervallSlut = slutDatum.Date.AddDays(1); 
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

            var avbokade = unitOfWork.BokningRepository
                .Find(b => b.Status == "Avbokad")
                .Select(b => b.BokningsNummer)
                .ToList();

            logiRader = logiRader.Where(r => r.BokningsNummer == null || !avbokade.Contains(r.BokningsNummer.Value)).ToList();
            utrustningRader = utrustningRader.Where(r => r.BokningsNummer == null || !avbokade.Contains(r.BokningsNummer.Value)).ToList();
            skidlektionRader = skidlektionRader.Where(r => r.BokningsNummer == null || !avbokade.Contains(r.BokningsNummer.Value)).ToList();
            konferensRader = konferensRader.Where(r => r.BokningsNummer == null || !avbokade.Contains(r.BokningsNummer.Value)).ToList();

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

            foreach (var rad in logiRader)
            {
                DateTime start = rad.StartDatum.Date;
                DateTime slut = rad.SlutDatum.Date;
                int totalaNätter = (slut - start).Days;
                int nätterInom = Overlapp(start, slut, intervallStart, intervallSlut);
                decimal intäkt = Andel(rad.LogiBelopp, nätterInom, totalaNätter);

                resultat.BokadeLogiNätter += nätterInom;
                resultat.IntäktLogi += intäkt;

                if (rad.LogiNummer != null && typPerLogi.ContainsKey(rad.LogiNummer))
                {
                    var alternativ = perAlternativ[typPerLogi[rad.LogiNummer]];
                    alternativ.BokadeNätter += nätterInom;
                    alternativ.Intäkt += intäkt;
                }
            }

            foreach (var rad in utrustningRader)
            {
                DateTime start = rad.StartDatum.Date;
                DateTime slut = rad.SlutDatum.Date.AddDays(1);
                int totalaDagar = (slut - start).Days;
                int dagarInom = Overlapp(start, slut, intervallStart, intervallSlut);

                resultat.IntäktUtrustning += Andel(rad.UtrustningBelopp, dagarInom, totalaDagar);

                if (rad.UtrustningNummer != null)
                {
                    resultat.BokadeUtrustningDagar += dagarInom;
                }
            }

            foreach (var rad in skidlektionRader)
            {
                resultat.IntäktSkidlektion += rad.SkidlektionBelopp;
                resultat.BokadeLektionsPlatser += rad.AntalPersoner;
            }

            foreach (var rad in konferensRader)
            {
                DateTime start = rad.StartTid.Date;
                DateTime slut = rad.SlutTid.Date.AddDays(1);
                int totalaDagar = (slut - start).Days;
                int dagarInom = Overlapp(start, slut, intervallStart, intervallSlut);

                resultat.IntäktKonferens += Andel(rad.KonferensBelopp, dagarInom, totalaDagar);
            }

            resultat.AntalBokningar = logiRader.Select(r => r.BokningsNummer)
                .Concat(utrustningRader.Select(r => r.BokningsNummer))
                .Concat(skidlektionRader.Select(r => r.BokningsNummer))
                .Concat(konferensRader.Select(r => r.BokningsNummer))
                .Where(nummer => nummer != null)
                .Distinct()
                .Count();

            resultat.TillgängligaLogiNätter = allaLogi.Count * antalDagarIIntervall;
            resultat.BeläggningLogiProcent = Procent(resultat.BokadeLogiNätter, resultat.TillgängligaLogiNätter);

            foreach (var alternativ in perAlternativ.Values)
            {
                alternativ.TillgängligaNätter = alternativ.AntalEnheter * antalDagarIIntervall;
                alternativ.BeläggningProcent = Procent(alternativ.BokadeNätter, alternativ.TillgängligaNätter);
                alternativ.Intäkt = Math.Round(alternativ.Intäkt, 2);
            }
            resultat.LogiPerAlternativ = perAlternativ.Values.OrderBy(a => a.Logialternativ).ToList();

            resultat.TillgängligaUtrustningDagar = antalUtrustning * antalDagarIIntervall;
            resultat.BeläggningUtrustningProcent = Procent(resultat.BokadeUtrustningDagar, resultat.TillgängligaUtrustningDagar);

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
        private int Overlapp(DateTime start, DateTime slut, DateTime intervallStart, DateTime intervallSlut)
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

        // Hämtar statistik för valt intervall och för en jämförelseperiod som användaren väljer
        public StatistikResultat HämtaStatistikMedJämförelse(DateTime startDatum, DateTime slutDatum,
                                                             DateTime jämförStartDatum, DateTime jämförSlutDatum)
        {
            StatistikResultat resultat = HamtaStatistik(startDatum, slutDatum);
            resultat.Föregående = HamtaStatistik(jämförStartDatum, jämförSlutDatum);

            // Koppla ihop logialternativen så att tabellen kan visa båda perioderna
            foreach (var rad in resultat.LogiPerAlternativ)
            {
                var föregåendeRad = resultat.Föregående.LogiPerAlternativ
                    .FirstOrDefault(f => f.Logialternativ == rad.Logialternativ);

                if (föregåendeRad != null)
                {
                    rad.FöregåendeBeläggningProcent = föregåendeRad.BeläggningProcent;
                    rad.FöregåendeIntäkt = föregåendeRad.Intäkt;
                }
            }

            return resultat;
        
        }

        #endregion
    }

    public class StatistikResultat
    {
        public DateTime StartDatum { get; set; }
        public DateTime SlutDatum { get; set; }

        public int AntalBokningar { get; set; }

        public decimal IntäktLogi { get; set; }
        public decimal IntäktUtrustning { get; set; }
        public decimal IntäktSkidlektion { get; set; }
        public decimal IntäktKonferens { get; set; }
        public decimal TotalIntäkt => IntäktLogi + IntäktUtrustning + IntäktSkidlektion + IntäktKonferens;

        public int BokadeLogiNätter { get; set; }
        public int TillgängligaLogiNätter { get; set; }
        public double BeläggningLogiProcent { get; set; }

        public List<LogiStatistikRad> LogiPerAlternativ { get; set; } = new List<LogiStatistikRad>();

        public int BokadeUtrustningDagar { get; set; }
        public int TillgängligaUtrustningDagar { get; set; }
        public double BeläggningUtrustningProcent { get; set; }

        public int AntalLektioner { get; set; }
        public int BokadeLektionsPlatser { get; set; }
        public int TillgängligaLektionsPlatser { get; set; }
        public double BeläggningSkidlektionProcent { get; set; }
        public StatistikResultat? Föregående { get; set; }
    }

    public class LogiStatistikRad
    {
        public string Logialternativ { get; set; } = string.Empty;
        public int AntalEnheter { get; set; }
        public int BokadeNätter { get; set; }
        public int TillgängligaNätter { get; set; }
        public double BeläggningProcent { get; set; }
        public decimal Intäkt { get; set; }
        public double FöregåendeBeläggningProcent { get; set; }
        public decimal FöregåendeIntäkt { get; set; }
    }
}