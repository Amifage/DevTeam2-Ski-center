using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class KonferensController
    {
        //Används i bokning /Sara
        public List<Konferens> HamtaAllaKonferenser()
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.KonferensRepository
                .GetAll()
                .ToList();
        }

        public List<Konferens> HamtaLedigaKonferenser(
            DateTime startTid,
            DateTime slutTid)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            var upptagnaKonferensNummer = unitOfWork.KonferensRadRepository
                .GetAll()
                .Where(r =>
                    r.StartTid < slutTid &&
                    r.SlutTid > startTid &&
                    r.KonferensNummer != null)
                .Select(r => r.KonferensNummer!)
                .ToHashSet();

            return unitOfWork.KonferensRepository
                .GetAll()
                .Where(k => !upptagnaKonferensNummer.Contains(k.KonferensNummer))
                .ToList();
        }
    }
}
