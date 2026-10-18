using System.Globalization;
using Datalager;
using Entitetslager;

namespace Servicelager
{
    public class PersonalController
    {
        private static readonly string[] DatumFormat =
        {
            "yyyy-MM-dd", "yyyy/MM/dd", "yyyyMMdd",
            "d/M/yyyy", "dd.MM.yyyy", "d.M.yyyy"
        };

        private static readonly string[] ExkluderadeRoller = { "marknad", "administrat" };

        private readonly UnitOfWork _unitOfWork;

        public PersonalController()
        {
            var dbContext = new SkiContext();
            _unitOfWork = new UnitOfWork(dbContext);
        }

        public List<Skidlektion> HamtaAktuellaSkidlektioner()
        {
            DateTime fran = DateTime.Today;
            DateTime till = fran.AddDays(14);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= fran && s.Datum < till)
                .OrderBy(s => s.Datum)
                .ThenBy(s => s.StartTid)
                .ToList();
        }

        public List<Skidlektion> HamtaSkidlektionerForDatum(DateTime datum)
        {
            DateTime fran = datum.Date;
            DateTime till = fran.AddDays(1);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.Datum >= fran && s.Datum < till)
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

        public bool HarOverlappandeLektion(int skidlektionNummer, int anstallningsNummer)
        {
            Skidlektion? lektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            if (lektion == null)
                return false;

            DateTime dag = lektion.Datum.Date;
            DateTime nastaDag = dag.AddDays(1);

            return _unitOfWork.SkidlektionRepository
                .Find(s => s.AnställningsNummer == anstallningsNummer &&
                           s.SkidlektionNummer != skidlektionNummer &&
                           s.Datum >= dag && s.Datum < nastaDag)
                .Any(s => s.StartTid < lektion.SlutTid && s.SlutTid > lektion.StartTid);
        }

        public bool UppdateraInstruktor(int skidlektionNummer, int nyttAnstallningsNummer)
        {
            Skidlektion? lektion = _unitOfWork.SkidlektionRepository
                .Find(s => s.SkidlektionNummer == skidlektionNummer)
                .FirstOrDefault();

            Personal? nyInstruktor = _unitOfWork.PersonalRepository
                .Find(p => p.AnställningsNummer == nyttAnstallningsNummer)
                .FirstOrDefault();

            if (lektion == null || nyInstruktor == null || !ArOrdinarieAnstalld(nyInstruktor))
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

        private static bool ArOrdinarieAnstalld(Personal personal)
        {
            return personal.Roll == null ||
                   !ExkluderadeRoller.Any(r => personal.Roll.Contains(r, StringComparison.OrdinalIgnoreCase));
        }
    }
}