using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class KonferensRad
    {
        public int KonferensRadNummer { get; set; }
        public DateTime SenastUppdaterad { get; set; }
        public DateTime StartTid { get; set; }
        public DateTime SlutTid { get; set; }
        public decimal KonferensBelopp { get; set; }
        public int AntalPersoner { get; set; }


        public string? KonferensNummer { get; set; }
        public virtual Konferens konferens { get; set; } //nav prop

        public int? BokningsNummer { get; set; }
        public virtual Bokning bokning { get; set; }//nav prop
    }
}
