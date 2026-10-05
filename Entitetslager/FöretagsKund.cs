using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    [Table("Företagskund")]
    public class FöretagsKund : Kund
    {
        public string FöretagsNamn { get; set; } 
        public decimal Rabatt { get; set; }
        public decimal Kredit { get; set; }
    }
}
