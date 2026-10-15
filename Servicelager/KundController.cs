using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitetslager;
using Datalager;

namespace Servicelager
{
    public class KundController
    {
        public void SkapaKund (Kund kund) 
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            _unitofwork.KundRepository.Add(kund);
            _unitofwork.Save();
        }

        public List<Kund> HamtaAllaKunder()
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());
            return _unitofwork.KundRepository.GetAll().ToList();
        }
        public void UppdateraKund(Kund kund)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            _unitofwork.KundRepository.Update(kund);
            _unitofwork.Save(); 
        }

        public void TaBortKund (Kund kund)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            _unitofwork.KundRepository.Remove(kund);
            _unitofwork.Save();
        }

        public void UppdateraPrivatkundersRabatt()
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            var allaKunder = _unitofwork.KundRepository.GetAll().OfType<PrivatKund>().ToList();
            var gransDatum = DateTime.Now.AddMonths(-12);

            foreach (var kund in allaKunder)
            {
                bool harNyligBokning = kund.Bokningar != null &&
                                        kund.Bokningar.Any(b => b.BokningsDatum >= gransDatum);

                if (harNyligBokning && kund.Rabatt != 8)
                {
                    kund.Rabatt = 8;
                    _unitofwork.KundRepository.Update(kund);
                }
            }

            _unitofwork.Save();
        }

        public void MinskaKredit(int kundNummer, decimal belopp)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            Kund? kund = _unitofwork.KundRepository
                .GetAll()
                .FirstOrDefault(k => k.KundNummer == kundNummer);

            if (kund == null)
                return;

            if (kund is PrivatKund privatKund)
            {
                privatKund.Kredit -= belopp;
                _unitofwork.KundRepository.Update(privatKund);
            }
            else if (kund is FöretagsKund företagsKund)
            {
                företagsKund.Kredit -= belopp;
                _unitofwork.KundRepository.Update(företagsKund);
            }

            _unitofwork.Save();
        }

        public void OkaKredit(int kundNummer, decimal belopp)
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());

            Kund? kund = _unitofwork.KundRepository
                .GetAll()
                .FirstOrDefault(k => k.KundNummer == kundNummer);

            if (kund == null)
                return;

            if (kund is PrivatKund privatKund)
            {
                privatKund.Kredit += belopp;
                _unitofwork.KundRepository.Update(privatKund);
            }
            else if (kund is FöretagsKund företagsKund)
            {
                företagsKund.Kredit += belopp;
                _unitofwork.KundRepository.Update(företagsKund);
            }

            _unitofwork.Save();
        }


    }
}
