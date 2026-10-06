using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    public class UtrustningPaketInnehåll
    {
        [Key]
        public int UtrustningPaketInnehållNummer { get; set; }

        public int Antal { get; set; }


        public int? UtrustningPaketNummer { get; set; }

        [ForeignKey(nameof(UtrustningPaketNummer))]
        public virtual UtrustningPaket? utrustningPaket { get; set; }


        public int? ArtikelTypNummer { get; set; }

        [ForeignKey(nameof(ArtikelTypNummer))]
        public virtual ArtikelTyp? artikelTyp { get; set; }
    }
}