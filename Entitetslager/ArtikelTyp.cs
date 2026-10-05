using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class ArtikelTyp
    {
       [Key] public int ArtikelTypNummer { get; set; }
        public string TypNamn { get; set; } = null!;
        public DateTime SenastUppdaterad { get; set; }

        // Navigational properties
        public virtual ICollection<Pris> Priser { get; set; }
            = new List<Pris>();

        public virtual ICollection<Logi> Logi { get; set; }
            = new List<Logi>();

        public virtual ICollection<Konferens> Konferenser { get; set; }
            = new List<Konferens>();

        public virtual ICollection<Skidlektion> Skidlektioner { get; set; }
            = new List<Skidlektion>();

        public virtual ICollection<Utrustning> Utrustningar { get; set; }
            = new List<Utrustning>();

        public virtual ICollection<UtrustningPaket> UtrustningPaket { get; set; }
            = new List<UtrustningPaket>();

        public virtual ICollection<UtrustningPaketInnehåll> UtrustningPaketInnehåll { get; set; }
            = new List<UtrustningPaketInnehåll>();
    }
}