using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class FöretagsKund : Kund
    {
        public string FöretagsNamn { get; set; } 
        public int? Rabatt { get; set; }
        public decimal? Kredit { get; set; }
    }
}
