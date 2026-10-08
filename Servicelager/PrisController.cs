using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalager;
using Entitetslager;
using System.Collections.Generic;
using System.Linq;

namespace Servicelager
{
    public class PrisController
    {
        public List<Pris> HämtaPriserFörArtikelTyp(int artikelTypNummer)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.PrisRepository
                .GetAll()
                .Where(p => p.ArtikelTypNummer == artikelTypNummer)
                .ToList();
        }

        public Pris? HämtaAktuelltPris(int artikelTypNummer, DateTime startDatum, DateTime slutDatum)
        {
            var priser = HämtaPriserFörArtikelTyp(artikelTypNummer);

            int antalDagar = (slutDatum.Date - startDatum.Date).Days;

            return priser
                .OrderBy(p => p.AntalDagar == antalDagar ? 0 : 1)
                .FirstOrDefault(p =>
                    p.AntalDagar == antalDagar ||
                    p.AntalDagar == null);
        }


        public decimal? BeräknaPrisFörPeriod(int artikelTypNummer, DateTime startDatum, DateTime slutDatum)
        {
            int antalDagar = (slutDatum.Date - startDatum.Date).Days;

            if (antalDagar <= 0)
                return null;

            List<Pris> priser = HämtaPriserFörArtikelTyp(artikelTypNummer)
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

    }
}
