using Datalager;
using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicelager
{
    public class KonferensController
    {
        private readonly UnitOfWork _unitOfWork;

        public KonferensController()
        {
            _unitOfWork = new UnitOfWork(new SkiContext());
        }

        public List<Konferens> HamtaAllaKonferenser()
        {
            return _unitOfWork.KonferensRepository
                .GetAll()
                .ToList();
        }

        public List<Konferens> HamtaLedigaKonferenser(DateTime startTid, DateTime slutTid)
        {
            HashSet<int> avbokadeBokningsNummer = _unitOfWork.BokningRepository
                .GetAll()
                .Where(b => b.Status == "Avbokad")
                .Select(b => b.BokningsNummer)
                .ToHashSet();

            HashSet<string> upptagnaKonferensNummer =
                _unitOfWork.KonferensRadRepository
                    .GetAll()
                    .Where(r =>
                        r.StartTid < slutTid &&
                        r.SlutTid > startTid &&
                        r.KonferensNummer != null &&
                        (!r.BokningsNummer.HasValue ||
                         !avbokadeBokningsNummer.Contains(r.BokningsNummer.Value)))
                    .Select(r => r.KonferensNummer!)
                    .ToHashSet();

            return _unitOfWork.KonferensRepository
                .GetAll()
                .Where(k => !upptagnaKonferensNummer.Contains(k.KonferensNummer))
                .ToList();
        }

        public List<Konferens> SokKonferens(string? sokNummer, string? sokStatus, string? sokStorlek)
        {
            var query = _unitOfWork.KonferensRepository
                .GetAll()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sokNummer))
            {
                query = query.Where(k =>
                    k.KonferensNummer.Contains(sokNummer));
            }

            if (!string.IsNullOrWhiteSpace(sokStatus))
            {
                query = query.Where(k =>
                    k.Status != null &&
                    k.Status.Contains(sokStatus));
            }

            if (!string.IsNullOrWhiteSpace(sokStorlek))
            {
                if (sokStorlek.Equals(
                    "Liten (max 20)",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(k =>
                        k.KonferensKapacitet <= 20);
                }
                else if (sokStorlek.Equals(
                    "Stor (max 30)",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(k =>
                        k.KonferensKapacitet > 20);
                }
            }

            return query.ToList();
        }

        public Konferens? HamtaKonferensMedNummer(string konferensNummer)
        {
            return _unitOfWork.KonferensRepository
                .GetAll()
                .FirstOrDefault(k =>
                    k.KonferensNummer == konferensNummer);
        }

        public bool UppdateraKonferensStatus(string konferensNummer, string nyStatus)
        {
            Konferens? konferens =
                HamtaKonferensMedNummer(konferensNummer);

            if (konferens == null)
                return false;

            konferens.Status = nyStatus;
            konferens.SenastUppdaterad = DateTime.Now;

            _unitOfWork.KonferensRepository.Update(konferens);
            _unitOfWork.Save();

            return true;
        }
    }
}