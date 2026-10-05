using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class UtrustningPaketInnehåll
    {
        [Key] public int UtrustningPaketInnehållNummer { get; set; }
        public int Antal {  get; set; }

        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }

    }
}
