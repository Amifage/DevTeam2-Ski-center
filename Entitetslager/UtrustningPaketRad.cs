using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    public class UtrustningPaketRad
    {
        [Key]
        public int UtrustningPaketRadNummer { get; set; }


        public string? UtrustningNummer { get; set; }

        [ForeignKey(nameof(UtrustningNummer))]
        public virtual Utrustning? utrustning { get; set; }


        public int? UtrustningRadNummer { get; set; }

        [ForeignKey(nameof(UtrustningRadNummer))]
        public virtual UtrustningRad? utrustningRad { get; set; }
    }
}