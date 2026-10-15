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

        //Används i bokning/Sara
        public int HämtaAntalLedigaPlatser(int skidlektionNummer)
        {
            Skidlektion? skidlektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (skidlektion == null)
                return 0;

            int bokadePlatser = HamtaAktivaRader()
                .Where(r => r.SkidlektionNummer == skidlektionNummer)
                .Sum(r => r.AntalPersoner);

            return Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
        }

        //Används i bokning/Sara
        public List<Skidlektion> HämtaTillgängligaSkidlektioner(DateTime datum)
        {
            var skidlektioner = _unitOfWork.SkidlektionRepository
                .GetAll()
                .Where(s => s.Datum.Date == datum.Date)
                .OrderBy(s => s.StartTid)
                .ToList();

            var artikelTyper = _unitOfWork.ArtikelTypRepository
                .GetAll()
                .ToList();

            var aktivaRader = HamtaAktivaRader();

            foreach (Skidlektion skidlektion in skidlektioner)
            {
                skidlektion.artikelTyp = artikelTyper
                    .FirstOrDefault(a => a.ArtikelTypNummer == skidlektion.ArtikelTypNummer);

                int bokadePlatser = aktivaRader
                    .Where(r => r.SkidlektionNummer == skidlektion.SkidlektionNummer)
                    .Sum(r => r.AntalPersoner);

                skidlektion.LedigaPlatser = Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
            }

            return skidlektioner
                .Where(s => s.LedigaPlatser > 0)
                .ToList();
        }

        private HashSet<int> HamtaAvbokadeBokningsNummer()
        {
            return _unitOfWork.BokningRepository
                .GetAll()
                .Where(b => b.Status == "Avbokad")
                .Select(b => b.BokningsNummer)
                .ToHashSet();
        }

        private List<SkidlektionRad> HamtaAktivaRader()
        {
            var avbokade = HamtaAvbokadeBokningsNummer();

            return _unitOfWork.SkidlektionRadRepository
                .GetAll()
                .Where(r => !r.BokningsNummer.HasValue ||
                            !avbokade.Contains(r.BokningsNummer.Value))
                .ToList();
        }
    }
}