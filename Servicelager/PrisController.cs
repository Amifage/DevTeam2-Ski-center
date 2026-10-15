using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class PrisController
    {
        public List<Pris> HamtaPriserForArtikelTyp(int artikelTypNummer)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.PrisRepository
                .GetAll()
                .Where(p => p.ArtikelTypNummer == artikelTypNummer)
                .ToList();
        }

        public Pris? HamtaAktuelltPris(int artikelTypNummer, DateTime startDatum, DateTime slutDatum)
        {
            var priser = HamtaPriserForArtikelTyp(artikelTypNummer);

            int antalDagar = (slutDatum.Date - startDatum.Date).Days;

            return priser
                .OrderBy(p => p.AntalDagar == antalDagar ? 0 : 1)
                .FirstOrDefault(p =>
                    p.AntalDagar == antalDagar ||
                    p.AntalDagar == null);
        }

        private Pris? HittaPris(List<Pris> priser, int prisRegelNummer, DateTime datum)
        {
            int vecka = ISOWeek.GetWeekOfYear(datum);
            int år = ISOWeek.GetYear(datum);

            return priser
                .Where(p => p.PrisRegelNummer == prisRegelNummer)
                .Where(p => !p.Vecka.HasValue || p.Vecka.Value == vecka)
                .Where(p => !p.År.HasValue || p.År.Value == år)
                .OrderBy(p => p.Vecka.HasValue ? 0 : 1)
                .FirstOrDefault();
        }

        public decimal? BeraknaLogiPris(int artikelTypNummer, DateTime startDatum, DateTime slutDatum)
        {
            if (slutDatum.Date <= startDatum.Date)
                return null;

            List<Pris> priser = HamtaPriserForArtikelTyp(artikelTypNummer);

            decimal totalt = 0;
            DateTime dag = startDatum.Date;

            while (dag < slutDatum.Date)
            {
                int kvarNatter = (slutDatum.Date - dag).Days;

                Pris? veckoPris = kvarNatter >= 7
                    ? HittaPris(priser, 1, dag)
                    : null;

                if (veckoPris != null)
                {
                    totalt += veckoPris.PrisBelopp;
                    dag = dag.AddDays(7);
                    continue;
                }

                int regel = dag.DayOfWeek == DayOfWeek.Friday || dag.DayOfWeek == DayOfWeek.Saturday
                    ? 3
                    : 2;

                Pris? nattPris = HittaPris(priser, regel, dag) ?? HittaPris(priser, 4, dag);

                if (nattPris == null)
                    return null;

                totalt += nattPris.PrisBelopp;
                dag = dag.AddDays(1);
            }

            return totalt;
        }

        public decimal? BeraknaPrisForPeriod(int artikelTypNummer, DateTime startDatum, DateTime slutDatum)
        {
            int antalDagar = (slutDatum.Date - startDatum.Date).Days;

            if (antalDagar <= 0)
                return null;

            List<Pris> priser = HamtaPriserForArtikelTyp(artikelTypNummer)
                .Where(p => p.AntalDagar.HasValue && p.AntalDagar.Value > 0)
                .ToList();

            if (!priser.Any())
                return null;

            decimal?[] billigastePris = new decimal?[antalDagar + 1];
            billigastePris[0] = 0;

            for (int dag = 1; dag <= antalDagar; dag++)
            {
                foreach (Pris pris in priser)
                {
                    int period = pris.AntalDagar!.Value;

                    if (period > dag || billigastePris[dag - period] == null)
                        continue;

                    decimal nyttPris =
                        billigastePris[dag - period]!.Value + pris.PrisBelopp;

                    if (billigastePris[dag] == null ||
                        nyttPris < billigastePris[dag])
                    {
                        billigastePris[dag] = nyttPris;
                    }
                }
            }

            return billigastePris[antalDagar];
        }

        //Pris för skidlektioner/Sara
        public Pris? HamtaPrisForSkidlektion(int artikelTypNummer, DateTime datum)
        {
            int veckodag = datum.DayOfWeek switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => 0
            };

            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.PrisRepository.GetAll()
                .FirstOrDefault(p =>
                    p.ArtikelTypNummer == artikelTypNummer &&
                    p.Veckodag == veckodag);
        }
    }
}