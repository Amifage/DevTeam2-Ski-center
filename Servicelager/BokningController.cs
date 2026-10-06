using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class BokningController
    {
        public void SkapaBokning(Bokning bokning)
        {
            using var unitOfWork = new UnitOfWork(new SkiContext());

            unitOfWork.BokningRepository.Add(bokning);
            unitOfWork.Save();
        }
    }
}