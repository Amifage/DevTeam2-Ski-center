using Datalager;
using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicelager
{
    public class LogiController
    {
        private readonly UnitOfWork _unitOfWork;


        public LogiController()
        {
            _unitOfWork = new UnitOfWork(new SkiContext());
        }
        public List<Logi> HamtaAllaLogi()
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.LogiRepository
                .GetAll()
                .ToList();
        }


        public List<string> HamtaUpptagnaLogi(
             DateTime startDatum,
             DateTime slutDatum)
        {
            using var unitOfWork =
                new UnitOfWork(new SkiContext());

            var avbokadeBokningsNummer =
                unitOfWork.BokningRepository
                    .GetAll()
                    .Where(b => b.Status == "Avbokad")
                    .Select(b => b.BokningsNummer)
                    .ToHashSet();

            return unitOfWork.LogiRadRepository
                .GetAll()
                .Where(r =>
                    r.LogiNummer != null &&
                    r.StartDatum < slutDatum &&
                    r.SlutDatum > startDatum &&
                    (!r.BokningsNummer.HasValue ||
                     !avbokadeBokningsNummer.Contains(
                         r.BokningsNummer.Value)))
                .Select(r => r.LogiNummer!)
                .Distinct()
                .ToList();
        }

        public List<Logi> SokLogi(string logiNummer, string status, string artikelTypNamn)
        {
            var resultat = _unitOfWork.LogiRepository.GetAll().ToList();

            if (!string.IsNullOrWhiteSpace(logiNummer))
            {
                resultat = resultat.Where(l => l.LogiNummer != null &&
                    l.LogiNummer.IndexOf(logiNummer, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                resultat = resultat.Where(l => l.Status != null &&
                    l.Status.IndexOf(status, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(artikelTypNamn))
            {
                var matchandeArtikelTypNummer = _unitOfWork.ArtikelTypRepository.GetAll()
                    .Where(at => at.TypNamn != null && at.TypNamn.Equals(artikelTypNamn, StringComparison.OrdinalIgnoreCase))
                    .Select(at => at.ArtikelTypNummer)
                    .ToList();

                resultat = resultat.Where(l => l.ArtikelTypNummer.HasValue &&
                    matchandeArtikelTypNummer.Contains(l.ArtikelTypNummer.Value)).ToList();
            }

            return resultat;
        }

        public Logi HamtaLogiMedArtikelTypNummer(string logiNummer)
        {
            return _unitOfWork.LogiRepository
                .Find(l => l.LogiNummer == logiNummer)
                .FirstOrDefault();
        }

        public List<ArtikelTyp> HamtaAllaArtikelTyper()
        {
            return _unitOfWork.ArtikelTypRepository.GetAll().ToList();
        }
       
        public void UppdateraLogi(Logi logi)
        {
            logi.SenastUppdaterad = DateTime.Now;
            _unitOfWork.LogiRepository.Update(logi);
            _unitOfWork.Save();
        }

    }
}
