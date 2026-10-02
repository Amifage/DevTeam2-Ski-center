using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Konferens
    {
        public string KonferensNummer { get; set; }
        public int KonferensKapacitet { get; set; }
        public string Status { get; set; }
        public DateTime SenastUppdaterad { get; set; }

        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
