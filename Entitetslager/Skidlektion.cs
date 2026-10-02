using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Skidlektion
    {
        public int SkidlektionNummer { get; set; }
        public string SkidlektionTyp { get; set; } 
        public int Kapacitet { get; set; }
        public DateTime SenastUppdaterad { get; set; }

        public int? AnställningsNummer { get; set; }
        public Personal Personal { get; set; }


        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
