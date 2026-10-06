using System;
using System.Collections.Generic;
using System.Linq;
using Entitetslager;
using Datalager;

namespace Servicelager
{
    public class UtrustningController
    {
        private readonly UnitOfWork _unitOfWork;

        public UtrustningController()
        {
            _unitOfWork = new UnitOfWork(new SkiContext());
        }

        public List<Utrustning> HamtaAllaUtrustningar()
        {
            return _unitOfWork.UtrustningRepository.GetAll().ToList();
        }

        public Utrustning HamtaUtrustningMedsArtikelnummer(string utrustningNummer)
        {
            return _unitOfWork.UtrustningRepository
                .Find(u => u.UtrustningNummer == utrustningNummer)
                .FirstOrDefault();
        }

        public List<Utrustning> SokUtrustning(string utrustningNummer, string status, string artikelTypNamn, string utrustningsPaketNamn)
        {
            // 1. Hämta alla utrustningar till minnet
            var resultat = _unitOfWork.UtrustningRepository.GetAll().ToList();

            // 2. Sökning i Utrustning på UtrustningNummer
            if (!string.IsNullOrWhiteSpace(utrustningNummer))
            {
                resultat = resultat.Where(u => u.UtrustningNummer != null &&
                    u.UtrustningNummer.IndexOf(utrustningNummer, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            // 3. Sökning i Utrustning på Status
            if (!string.IsNullOrWhiteSpace(status))
            {
                resultat = resultat.Where(u => u.Status != null &&
                    u.Status.IndexOf(status, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            // 4. Sökning i ArtikelTyp på TypNamn
            if (!string.IsNullOrWhiteSpace(artikelTypNamn))
            {
                var matchandeArtikelTypNummer = _unitOfWork.ArtikelTypRepository.GetAll()
                    .Where(at => at.TypNamn != null && at.TypNamn.Equals(artikelTypNamn, StringComparison.OrdinalIgnoreCase))
                    .Select(at => at.ArtikelTypNummer)
                    .ToList();

                resultat = resultat.Where(u => u.ArtikelTypNummer.HasValue &&
                    matchandeArtikelTypNummer.Contains(u.ArtikelTypNummer.Value)).ToList();
            }

            // 5. Sökning i UtrustningPaket på UtrustningPaketNamn
            if (!string.IsNullOrWhiteSpace(utrustningsPaketNamn))
            {
                // Hitta de ArtikelTypNummer som hör till paket med matchande UtrustningPaketNamn
                var matchandePaketArtikelTypNummer = _unitOfWork.UtrustningPaketRepository.GetAll()
                    .Where(p => p.UtrustningPaketNamn != null && p.UtrustningPaketNamn.Equals(utrustningsPaketNamn, StringComparison.OrdinalIgnoreCase))
                    .Where(p => p.ArtikelTypNummer.HasValue)
                    .Select(p => p.ArtikelTypNummer.Value)
                    .ToList();

                resultat = resultat.Where(u => u.ArtikelTypNummer.HasValue &&
                    matchandePaketArtikelTypNummer.Contains(u.ArtikelTypNummer.Value)).ToList();
            }

            return resultat;
        }

        public List<ArtikelTyp> HamtaAllaArtikelTyper()
        {
            return _unitOfWork.ArtikelTypRepository.GetAll().ToList();
        }

        public List<UtrustningPaket> HamtaAllaUtrustningsPaket()
        {
            return _unitOfWork.UtrustningPaketRepository.GetAll().ToList();
        }

        public bool UppdateraUtrustningsStatus(string utrustningNummer, string nyStatus)
        {
            var utrustning = HamtaUtrustningMedsArtikelnummer(utrustningNummer);
            if (utrustning != null)
            {
                utrustning.Status = nyStatus;
                utrustning.SenastUppdaterad = DateTime.Now;
                _unitOfWork.UtrustningRepository.Update(utrustning);
                _unitOfWork.Save();
                return true;
            }
            return false;
        }

        public void LaggTillUtrustning(Utrustning nyUtrustning)
        {
            nyUtrustning.SenastUppdaterad = DateTime.Now;
            _unitOfWork.UtrustningRepository.Add(nyUtrustning);
            _unitOfWork.Save();
        }

        public void UppdateraUtrustning(Utrustning utrustning)
        {
            utrustning.SenastUppdaterad = DateTime.Now;
            _unitOfWork.UtrustningRepository.Update(utrustning);
            _unitOfWork.Save();
        }
    }
}