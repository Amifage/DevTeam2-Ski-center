using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Pris
    {
        [Key] public int PrisNummer { get; set; }
        public  int ArtikelNummer { get; set; }
        public string PrisRegelNummer { get; set; }
        public decimal PrisBelopp { get; set; }
        public int? AntalDagar { get; set; }
        public int? Vecka { get; set; }
        public int? År { get; set; }
        public int? Veckodag { get; set; }


        public virtual ArtikelTyp ArtikelTyp { get; set; } 
        public virtual PrisRegel PrisRegel { get; set; }
    }
}
