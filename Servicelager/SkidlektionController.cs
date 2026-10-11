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

        //Används i bokning/Sara
        public int HämtaAntalLedigaPlatser(int skidlektionNummer)
        {
            Skidlektion? skidlektion = _unitOfWork.SkidlektionRepository
                .GetAll()
                .FirstOrDefault(s => s.SkidlektionNummer == skidlektionNummer);

            if (skidlektion == null)
                return 0;

            int bokadePlatser = _unitOfWork.SkidlektionRadRepository
                .GetAll()
                .Where(r => r.SkidlektionNummer == skidlektionNummer)
                .Sum(r => r.AntalPersoner);

            return Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
        }
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

            foreach (Skidlektion skidlektion in skidlektioner)
            {
                skidlektion.artikelTyp = artikelTyper.FirstOrDefault(a => a.ArtikelTypNummer == skidlektion.ArtikelTypNummer);
            }

            var bokadeRader = _unitOfWork.SkidlektionRadRepository
                .GetAll()
                .ToList();

            foreach (Skidlektion skidlektion in skidlektioner)
            {
                int bokadePlatser = bokadeRader
                    .Where(r => r.SkidlektionNummer == skidlektion.SkidlektionNummer)
                    .Sum(r => r.AntalPersoner);

                skidlektion.LedigaPlatser =
                    Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
            }

            return skidlektioner
                .Where(s => s.LedigaPlatser > 0)
                .ToList();
        }
    }
}