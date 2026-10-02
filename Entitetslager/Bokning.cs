using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Bokning
    {
        public int BokningsNummer { get; set; }
        public DateTime BokningsDatum { get; set; }
        public string Status { get; set; }
        public DateTime SenastUppdaterad { get; set; }

        public List<KonferensRad>? KonferansRader { get; set; }
        public List<LogiRad>? LogiRader { get; set; }
        public List<UtrustningRad>? UtrustningRader { get; set; }
        public List<SkidlektionRad>? SkidlektionRader { get; set; }

        public int? KundNummer { get; set; }
        public virtual Kund kund { get; set; }

    }
}
