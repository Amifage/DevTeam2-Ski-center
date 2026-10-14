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

        // Lektioner de kommande två veckorna (idag + 14 dagar), sorterade på datum och starttid
        public IEnumerable<Skidlektion> HämtaAktuellaSkidlektioner()
        {
            DateTime från = DateTime.Today;
            DateTime till = från.AddDays(14);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= från && s.Datum < till)
                .OrderBy(s => s.Datum)
                .ThenBy(s => s.StartTid)
                .ToList();
        }

        // Alla lektioner för ett enskilt datum (oavsett hur långt fram det ligger)
        public IEnumerable<Skidlektion> HämtaSkidlektionerFörDatum(DateTime datum)
        {
            DateTime från = datum.Date;
            DateTime till = från.AddDays(1);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= från && s.Datum < till)
                .OrderBy(s => s.StartTid)
                .ToList();
        }

        // Anställda som har minst en skidlektion (används i filtret)
        public IEnumerable<Personal> HämtaInstruktörer()
        {
            var anställningsNummer = _unitOfWork.SkidlektionRepository
                .GetAll()
                .Where(s => s.AnställningsNummer.HasValue)
                .Select(s => s.AnställningsNummer!.Value)
                .Distinct()
                .ToList();

            return _unitOfWork.PersonalRepository
                .Find(p => anställningsNummer.Contains(p.AnställningsNummer))
                .OrderBy(p => p.Förnamn)
                .ToList();
        }

        // NYTT: alla anställda (används i rullistan när man byter instruktör)
        public IEnumerable<Personal> HämtaAllaAnställda()
        {
            return _unitOfWork.PersonalRepository
                .GetAll()
                .OrderBy(p => p.Förnamn)
                .ThenBy(p => p.Efternamn)
                .ToList();
        }

        // NYTT: byter instruktör på en lektion.
        // Returnerar null om det gick bra, annars ett felmeddelande att visa för användaren.
        public string? ÄndraInstruktör(int skidlektionNummer, int nyttAnställningsNummer)
        {
            Skidlektion? lektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (lektion == null)
                return "Lektionen finns inte längre.";

            if (lektion.AnställningsNummer == nyttAnställningsNummer)
                return "Den anställda har redan den här lektionen.";

            // Kontrollera att den nya instruktören inte har en annan lektion som överlappar i tid
            DateTime dag = lektion.Datum.Date;
            DateTime nästaDag = dag.AddDays(1);

            bool upptagen = _unitOfWork.SkidlektionRepository
                .Find(s => s.AnställningsNummer == nyttAnställningsNummer &&
                           s.SkidlektionNummer != skidlektionNummer &&
                           s.Datum >= dag && s.Datum < nästaDag)
                .Any(s => s.StartTid < lektion.SlutTid && s.SlutTid > lektion.StartTid);

            if (upptagen)
                return "Den anställda har redan en annan lektion som överlappar i tid.";

            int? gammaltNummer = lektion.AnställningsNummer;
            DateTime gammalUppdatering = lektion.SenastUppdaterad;

            try
            {
                // Lektionen är redan spårad av EF, så Update() behövs inte (och skulle markera
                // även relaterade objekt som ändrade). Det räcker att ändra och spara.
                lektion.AnställningsNummer = nyttAnställningsNummer;
                lektion.SenastUppdaterad = DateTime.Now;
                _unitOfWork.Save();
                return null;
            }
            catch
            {
                // Ångra ändringen i minnet så att listan inte visar något som inte sparats
                lektion.AnställningsNummer = gammaltNummer;
                lektion.SenastUppdaterad = gammalUppdatering;
                throw;
            }
        }

        // Används i bokning/Sara
        public int HämtaAntalLedigaPlatser(int skidlektionNummer)
        {
            Skidlektion? skidlektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (skidlektion == null)
                return 0;

            int bokadePlatser = HämtaAktivaRader()
                .Where(r => r.SkidlektionNummer == skidlektionNummer)
                .Sum(r => r.AntalPersoner);

            return Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
        }

        // Används i bokning/Sara
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

            var aktivaRader = HämtaAktivaRader();

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

        // Bokningsnummer för bokningar som är avbokade
        private HashSet<int> HämtaAvbokadeBokningsNummer()
        {
            return _unitOfWork.BokningRepository
                .GetAll()
                .Where(b => b.Status == "Avbokad")
                .Select(b => b.BokningsNummer)
                .ToHashSet();
        }

        // Lektionsrader som räknas som bokade (ingen bokning kopplad, eller bokningen är inte avbokad)
        private List<SkidlektionRad> HämtaAktivaRader()
        {
            var avbokade = HämtaAvbokadeBokningsNummer();

            return _unitOfWork.SkidlektionRadRepository
                .GetAll()
                .Where(r => !r.BokningsNummer.HasValue ||
                            !avbokade.Contains(r.BokningsNummer.Value))
                .ToList();
        }
    }
}