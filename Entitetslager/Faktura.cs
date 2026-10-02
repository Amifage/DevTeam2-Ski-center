using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Faktura
    {
        public int FakturaNummer { get; set; }

        public int BokningsNummer { get; set; }

        public string Status { get; set; } = null!;

        public string FakturaTyp { get; set; } = null!;

        public DateTime Fakturadatum { get; set; }

        public DateTime Förfallodatum { get; set; }

        public string? Anteckning { get; set; }


        // Navigation property
        public virtual Bokning bokning { get; set; }
    }
}
