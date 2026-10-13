using Datalager;
using Entitetslager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Servicelager
{
    public class FakturaController
    {
        private readonly UnitOfWork _unitOfWork;

        public FakturaController() 
        {
            _unitOfWork = new UnitOfWork(new SkiContext());

        }

        public List<Faktura> HämtaAllaFakturor()
        {
            using var _unitofwork = new UnitOfWork(new SkiContext());
            return _unitofwork.FakturaRepository.GetQueryable() 
              .Include(f => f.bokning)
             .ThenInclude(b => b.kund) 
              .ToList(); 
        }

        public void UppdateraFaktura (Faktura faktura)
        {
            _unitOfWork.FakturaRepository.Update(faktura);
            _unitOfWork.Save();
        }

      


    }
}
