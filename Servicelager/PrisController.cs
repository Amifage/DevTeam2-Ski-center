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
    }
}
