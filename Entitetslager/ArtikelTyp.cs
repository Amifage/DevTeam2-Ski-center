using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class ArtikelTyp
    {
        public int ArtikelTypNummer { get; set; }
        public string TypNamn { get; set; } = null!;
        public DateTime SenastUppdaterad { get; set; }


        // Navigational properties
        public ICollection<Pris> Priser { get; set; }
            = new List<Pris>();

        public ICollection<Logi> Logi { get; set; }
            = new List<Logi>();

        public ICollection<Konferens> Konferenser { get; set; }
            = new List<Konferens>();

        public ICollection<Skidlektion> Skidlektioner { get; set; }
            = new List<Skidlektion>();

        public ICollection<Utrustning> Utrustningar { get; set; }
            = new List<Utrustning>();

        public ICollection<UtrustningPaket> UtrustningPaket { get; set; }
            = new List<UtrustningPaket>();

        public ICollection<UtrustningPaketInnehåll> UtrustningPaketInnehåll { get; set; }
            = new List<UtrustningPaketInnehåll>();
    }
}
