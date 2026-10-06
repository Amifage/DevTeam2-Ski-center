using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    [Table("Privatkund")]
    public class PrivatKund : Kund
    {
        public string Förnamn { get; set; }
        public string Efternamn { get; set; } 
        public decimal Rabatt { get; set; }
        public decimal Kredit { get; set; }


        public string DisplayText => $"{KundNummer} | {KundTyp} | {Epost} | {Förnamn} | {Efternamn}";
    }
}
