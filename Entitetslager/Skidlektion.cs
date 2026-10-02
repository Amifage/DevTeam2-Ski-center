using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class Skidlektion
    {
        public int SkidLektionsNummer { get; set; }
        public string SkidLektionsTyp { get; set; } 
        public Personal Personal { get; set; } 
        public int Kapacitet { get; set; }
        public DateTime SenastUppdaterad { get; set; }


        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
