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

        public List<Kund> HämtaAllaKunder()
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

    }
}
