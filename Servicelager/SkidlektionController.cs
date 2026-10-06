using System.Collections.Generic;
using System.Linq;
using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class SkidlektionController
    {
        private readonly UnitOfWork _unitOfWork;

        public SkidlektionController()
        {
            
            var dbContext = new SkiContext();
            _unitOfWork = new UnitOfWork(dbContext);
        }

        public IEnumerable<Skidlektion> HämtaAllaSkidlektioner()
        {
           
            return _unitOfWork.SkidlektionRepository.GetAll();
            
        }
    }
}