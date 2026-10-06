using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Bokning
    {
        [Key] public int BokningsNummer { get; set; }
        public DateTime BokningsDatum { get; set; }
        public string Status { get; set; }

        public virtual List<KonferensRad>? KonferansRader { get; set; }
        public virtual List<LogiRad>? LogiRader { get; set; }
        public virtual List<UtrustningRad>? UtrustningRader { get; set; }
        public virtual List<SkidlektionRad>? SkidlektionRader { get; set; }


        public int? KundNummer { get; set; }
        [ForeignKey(nameof(KundNummer))]
        public virtual Kund kund { get; set; }

    }
}
