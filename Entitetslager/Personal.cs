using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class Personal
    {
        public int AnställningsNummer { get; set; }
        public string Roll { get; set; } = null!;
        public string Förnamn { get; set; } = null!;
        public string Efternamn { get; set; } = null!;
        public string Epost { get; set; } = null!;
        public DateTime SenastUppdaterad { get; set; }
    }
}
