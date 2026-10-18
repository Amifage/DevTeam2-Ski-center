using Datalager;
using Entitetslager;
using System.Globalization;

namespace Servicelager
{
    public class SkidlektionController
    {
        private static readonly string[] DatumFormat =
        {
            "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd",
            "d/M/yyyy", "dd.MM.yyyy", "d.M.yyyy"
        };

        private readonly UnitOfWork _unitOfWork;

        public SkidlektionController()
        {
            _unitOfWork = new UnitOfWork(new SkiContext());
        }

        public List<Skidlektion> HamtaAktuellaSkidlektioner()
        {
            DateTime från = DateTime.Today;
            DateTime till = från.AddDays(14);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= från && s.Datum < till)
                .OrderBy(s => s.Datum)
                .ThenBy(s => s.StartTid)
                .ToList();
        }

        public List<Skidlektion> HamtaSkidlektionerForDatum(DateTime datum)
        {
            DateTime från = datum.Date;
            DateTime till = från.AddDays(1);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= från && s.Datum < till)
                .OrderBy(s => s.StartTid)
                .ToList();
        }

        public List<Skidlektion> SokSkidlektioner(string datum, string status, string artikelTypNamn, int? anstallningsNummer)
        {
            List<Skidlektion> resultat;
            string datumText = datum?.Trim() ?? string.Empty;

            if (DateTime.TryParseExact(datumText, DatumFormat, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out DateTime sokDatum))
            {
                resultat = HamtaSkidlektionerForDatum(sokDatum);
            }
            else
            {
                resultat = HamtaAktuellaSkidlektioner();

                if (datumText != string.Empty)
                {
                    resultat = resultat.Where(s =>
                        s.Datum.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture).Contains(datumText)).ToList();
                }
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                resultat = resultat.Where(s => s.Status != null &&
                    s.Status.IndexOf(status, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            if (!string.IsNullOrWhiteSpace(artikelTypNamn))
            {
                var matchandeArtikelTypNummer = _unitOfWork.ArtikelTypRepository.GetAll()
                    .Where(at => at.TypNamn != null && at.TypNamn.Equals(artikelTypNamn, StringComparison.OrdinalIgnoreCase))
                    .Select(at => at.ArtikelTypNummer)
                    .ToList();

                resultat = resultat.Where(s => s.ArtikelTypNummer.HasValue &&
                    matchandeArtikelTypNummer.Contains(s.ArtikelTypNummer.Value)).ToList();
            }

            if (anstallningsNummer.HasValue)
            {
                resultat = resultat.Where(s => s.AnställningsNummer == anstallningsNummer.Value).ToList();
            }

            return resultat;
        }

        public List<ArtikelTyp> HamtaAllaArtikelTyper()
        {
            return _unitOfWork.ArtikelTypRepository.GetAll().ToList();
        }

        public List<Personal> HamtaInstruktorer()
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

        public List<Personal> HamtaAllaAnstallda()
        {
            return _unitOfWork.PersonalRepository
                .GetAll()
                .OrderBy(p => p.Förnamn)
                .ThenBy(p => p.Efternamn)
                .ToList();
        }

        public bool HarOverlappandeLektion(int skidlektionNummer, int anstallningsNummer)
        {
            Skidlektion? lektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (lektion == null)
                return false;

            DateTime dag = lektion.Datum.Date;
            DateTime nästaDag = dag.AddDays(1);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.AnställningsNummer == anstallningsNummer &&
                           s.SkidlektionNummer != skidlektionNummer &&
                           s.Datum >= dag && s.Datum < nästaDag)
                .Any(s => s.StartTid < lektion.SlutTid && s.SlutTid > lektion.StartTid);
        }

        public bool UppdateraInstruktor(int skidlektionNummer, int nyttAnstallningsNummer)
        {
            Skidlektion? lektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (lektion == null)
                return false;

            int? gammaltNummer = lektion.AnställningsNummer;
            DateTime gammalUppdatering = lektion.SenastUppdaterad;

            try
            {
                lektion.AnställningsNummer = nyttAnstallningsNummer;
                lektion.SenastUppdaterad = DateTime.Now;
                _unitOfWork.Save();
                return true;
            }
            catch
            {
                lektion.AnställningsNummer = gammaltNummer;
                lektion.SenastUppdaterad = gammalUppdatering;
                return false;
            }
        }


        //Bokning
        public int HamtaAntalLedigaPlatser(int skidlektionNummer)
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

        public List<Skidlektion> HamtaTillgangligaSkidlektioner(DateTime datum)
        {
            List<Skidlektion> skidlektioner =
                HamtaSkidlektionerForDatum(datum);

            List<ArtikelTyp> artikelTyper = _unitOfWork.ArtikelTypRepository
                .GetAll()
                .ToList();

            List<SkidlektionRad> aktivaRader = HamtaAktivaRader();

            foreach (Skidlektion skidlektion in skidlektioner)
            {
                skidlektion.artikelTyp = artikelTyper
                    .FirstOrDefault(a =>
                        a.ArtikelTypNummer == skidlektion.ArtikelTypNummer);

                int bokadePlatser = aktivaRader
                    .Where(r =>
                        r.SkidlektionNummer == skidlektion.SkidlektionNummer)
                    .Sum(r => r.AntalPersoner);

                skidlektion.LedigaPlatser =
                    Math.Max(0, skidlektion.Kapacitet - bokadePlatser);
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

        public void UppdateraStatusEfterBokning(int skidlektionNummer)
        {
            Skidlektion? skidlektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (skidlektion == null)
                return;

            int ledigaPlatser = HamtaAntalLedigaPlatser(skidlektionNummer);

            skidlektion.Status = ledigaPlatser == 0
                ? "Fullbokad"
                : "Tillgänglig";

            skidlektion.SenastUppdaterad = DateTime.Now;

            _unitOfWork.SkidlektionRepository.Update(skidlektion);
            _unitOfWork.Save();
        }
    }
}