using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class UtrustningPaketRad
    {
        [Key] public int UtrustningPaketRadNummer { get; set; }


        public string? UtrustningNummer { get; set; }
        public virtual Utrustning utrustning { get; set; } //nav prop

        public int? UtrustningRadNummer { get; set; }
        public virtual UtrustningRad utrustningRad { get; set; }
    }
}
