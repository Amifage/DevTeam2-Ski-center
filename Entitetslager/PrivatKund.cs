using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class PrivatKund : Kund
    {
        public string Förnamn { get; set; }
        public string Efternamn { get; set; } 
        public int? Rabatt { get; set; }
        public int? Kredit { get; set; }
    }
}
