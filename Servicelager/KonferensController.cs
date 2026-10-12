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

        public List<Konferens> SokKonferens(string? sokNummer, string? sokStatus, string? sokStorlek)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            var query = unitOfWork.KonferensRepository.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(sokNummer))
            {
                query = query.Where(k => k.KonferensNummer.Contains(sokNummer));
            }

            if (!string.IsNullOrWhiteSpace(sokStatus))
            {
                query = query.Where(k => k.Status != null && k.Status.Contains(sokStatus));
            }

            if (!string.IsNullOrWhiteSpace(sokStorlek))
            {
                if (sokStorlek.Equals("Liten (max 20)", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(k => k.KonferensKapacitet <= 20);
                }
                else if (sokStorlek.Equals("Stor (max 30)", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(k => k.KonferensKapacitet > 20); // eller k.KonferensKapacitet == 30
                }
            }

            return query.ToList();
        }
        public Konferens? HamtaKonferensMedNummer(string konferensNummer)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            return _unitofwork.KonferensRepository
                .GetAll()
                .FirstOrDefault(k => k.KonferensNummer == konferensNummer);
        }

        public bool UppdateraKonferensStatus(string konferensNummer, string nyStatus)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            var konferens = HamtaKonferensMedNummer(konferensNummer);
            if (konferens != null)
            {
                konferens.Status = nyStatus;
                konferens.SenastUppdaterad = DateTime.Now;
                _unitofwork.KonferensRepository.Update(konferens);
                _unitofwork.Save();
                return true;
            }
            return false;

        }
    }
}
