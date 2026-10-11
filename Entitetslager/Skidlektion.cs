using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    [Table("Skidlektion")]
    public class Skidlektion
    {
        [Key]
        public int SkidlektionNummer { get; set; }

        public DateTime Datum { get; set; }

        public TimeSpan StartTid { get; set; }

        public TimeSpan SlutTid { get; set; }

        public string Status { get; set; }

        [Column("SkidlektionKapacitet")]
        public int Kapacitet { get; set; }

        public DateTime SenastUppdaterad { get; set; }

        public int? AnställningsNummer { get; set; }

        [ForeignKey("AnställningsNummer")]
        public virtual Personal Personal { get; set; }

        public int? ArtikelTypNummer { get; set; }

        [ForeignKey("ArtikelTypNummer")]
        public virtual ArtikelTyp artikelTyp { get; set; }


        [NotMapped]
        public int LedigaPlatser { get; set; }

        [NotMapped]
        public string DisplayText =>
                        $"{artikelTyp?.TypNamn ?? "Skidlektion"} | " +
                        $"{Datum:yyyy-MM-dd} | " +
                        $"{StartTid:hh\\:mm} - {SlutTid:hh\\:mm} | " +
                        $"{LedigaPlatser} platser kvar";
    }
}