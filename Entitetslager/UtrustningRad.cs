using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class UtrustningRad
    {
        public int UtrustningRadNummer { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime SlutDatum { get; set; }
        public decimal UtrustningBelopp { get; set; }
        public DateTime SenastUppdaterad { get; set; }


        public int? UtrustningPaketNummer { get; set; }
        public virtual UtrustningPaket utrustningPaket { get; set; } // nav prop

        public string? UtrustningNummer { get; set; }
        public virtual Utrustning utrustning { get; set; } //nav prop

        public int? BokningsNummer { get; set; }
        public virtual Bokning bokning { get; set; } //nav prop
    }
}
