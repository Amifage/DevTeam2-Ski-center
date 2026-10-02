using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class UtrustningPaket
    {
        public string UtrustningPaketNummer { get; set; }
        public string UtrustningPaketNamn { get; set; }


        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
