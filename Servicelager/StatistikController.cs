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

            int antalLogi = unitOfWork.LogiRepository.GetAll().Count();
            int antalUtrustning = unitOfWork.UtrustningRepository.GetAll().Count();

            var resultat = new StatistikResultat
            {
                StartDatum = intervallStart,
                SlutDatum = slutDatum.Date
            };

            // Logi: räknas i nätter
            foreach (var rad in logiRader)
            {
                DateTime start = rad.StartDatum.Date;
                DateTime slut = rad.SlutDatum.Date;
                int totalaNätter = (slut - start).Days;
                int nätterInom = Överlapp(start, slut, intervallStart, intervallSlut);

                resultat.BokadeLogiNätter += nätterInom;
                resultat.IntäktLogi += Andel(rad.LogiBelopp, nätterInom, totalaNätter);
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

            // Skidlektioner: en dag, hela beloppet räknas
            foreach (var rad in skidlektionRader)
            {
                resultat.IntäktSkidlektion += rad.SkidlektionBelopp;
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

            // Beläggning
            resultat.TillgängligaLogiNätter = antalLogi * antalDagarIIntervall;
            resultat.BeläggningLogiProcent = Procent(resultat.BokadeLogiNätter, resultat.TillgängligaLogiNätter);

            resultat.TillgängligaUtrustningDagar = antalUtrustning * antalDagarIIntervall;
            resultat.BeläggningUtrustningProcent = Procent(resultat.BokadeUtrustningDagar, resultat.TillgängligaUtrustningDagar);

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

        public decimal IntäktLogi { get; set; }
        public decimal IntäktUtrustning { get; set; }
        public decimal IntäktSkidlektion { get; set; }
        public decimal IntäktKonferens { get; set; }
        public decimal TotalIntäkt => IntäktLogi + IntäktUtrustning + IntäktSkidlektion + IntäktKonferens;

        public int BokadeLogiNätter { get; set; }
        public int TillgängligaLogiNätter { get; set; }
        public double BeläggningLogiProcent { get; set; }

        public int BokadeUtrustningDagar { get; set; }
        public int TillgängligaUtrustningDagar { get; set; }
        public double BeläggningUtrustningProcent { get; set; }
    }
}