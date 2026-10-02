using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class FöretagsKund : Kund
    {
        public string FöretagsNamn { get; set; } = null!;
        public int? Rabatt { get; set; }
        public decimal? Kredit { get; set; }
    }
}
