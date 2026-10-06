using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Pris
    {
        [Key] public int PrisNummer { get; set; }
        public  int ArtikelTypNummer { get; set; }
        public int PrisRegelNummer { get; set; }
        public decimal PrisBelopp { get; set; }
        public int? AntalDagar { get; set; }
        public int? Vecka { get; set; }
        public int? År { get; set; }
        public int? Veckodag { get; set; }


        [ForeignKey(nameof(ArtikelTypNummer))]
        public virtual ArtikelTyp ArtikelTyp { get; set; }


        [ForeignKey(nameof(PrisRegelNummer))]
        public virtual PrisRegel PrisRegel { get; set; }
    }
}
