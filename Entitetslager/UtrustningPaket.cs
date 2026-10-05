using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class UtrustningPaket
    {
        [Key] public string UtrustningPaketNummer { get; set; }
        public string UtrustningPaketNamn { get; set; }


        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
