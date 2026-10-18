using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Faktura
    {
        [Key] public int FakturaNummer { get; set; }

        public int BokningsNummer { get; set; }

        public string Status { get; set; } = null!;

        public string FakturaTyp { get; set; } = null!;

        public DateTime Fakturadatum { get; set; }

        public DateTime Förfallodatum { get; set; }

        public string? Anteckning { get; set; }

        public decimal Belopp { get; set; }


        // Navigation property
        [ForeignKey("BokningsNummer")]
        public virtual Bokning bokning { get; set; }
    }
}
