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
        public List<Logi> HämtaAllaLogi()
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            return unitOfWork.LogiRepository
                .GetAll()
                .ToList();
        }

    }
}
