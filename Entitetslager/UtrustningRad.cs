using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    public class UtrustningRad
    {
        [Key]
        public int UtrustningRadNummer { get; set; }

        public DateTime StartDatum { get; set; }

        public DateTime SlutDatum { get; set; }

        public decimal UtrustningBelopp { get; set; }

        public DateTime SenastUppdaterad { get; set; }


        public int? UtrustningPaketNummer { get; set; }

        [ForeignKey(nameof(UtrustningPaketNummer))]
        public virtual UtrustningPaket? utrustningPaket { get; set; }


        public string? UtrustningNummer { get; set; }

        [ForeignKey(nameof(UtrustningNummer))]
        public virtual Utrustning? utrustning { get; set; }


        public int? BokningsNummer { get; set; }

        [ForeignKey(nameof(BokningsNummer))]
        public virtual Bokning? bokning { get; set; }


        public virtual List<UtrustningPaketRad> UtrustningPaketRader { get; set; } = new List<UtrustningPaketRad>();


        [NotMapped]
        public string? UtrustningDisplayText { get; set; }
    }
}