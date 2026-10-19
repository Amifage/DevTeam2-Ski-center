using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class PersonalController
    {
        private static readonly string[] ExkluderadeRoller = { "marknad", "administrat" };

        private readonly UnitOfWork _unitOfWork;

        public PersonalController()
        {
            var dbContext = new SkiContext();
            _unitOfWork = new UnitOfWork(dbContext);
        }

        public List<Personal> HamtaInstruktorer()
        {
            var anstallningsNummer = _unitOfWork.SkidlektionRepository
                .GetAll()
                .Where(s => s.AnställningsNummer.HasValue)
                .Select(s => s.AnställningsNummer!.Value)
                .Distinct()
                .ToList();

            return _unitOfWork.PersonalRepository
                .Find(p => anstallningsNummer.Contains(p.AnställningsNummer))
                .OrderBy(p => p.Förnamn)
                .ToList();
        }

        public List<Personal> HamtaAllaAnstallda()
        {
            return _unitOfWork.PersonalRepository
                .GetAll()
                .Where(ArOrdinarieAnstalld)
                .OrderBy(p => p.Förnamn)
                .ThenBy(p => p.Efternamn)
                .ToList();
        }

        public static bool ArOrdinarieAnstalld(Personal personal)
        {
            return personal.Roll == null ||
                   !ExkluderadeRoller.Any(r => personal.Roll.Contains(r, StringComparison.OrdinalIgnoreCase));
        }
    }
}