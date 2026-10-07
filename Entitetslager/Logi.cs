using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    public class Logi
    {
        [Key] public string LogiNummer { get; set; }
        public int? LogiKapacitet { get; set; }
        public string Status { get; set; }
        public DateTime SenastUppdaterad { get; set; }
        public int? AntalRum { get; set; }
        public int? Storlek { get; set; }
        public string? Faciliteter { get; set; }


        public int? ArtikelTypNummer { get; set; }
        [ForeignKey("ArtikelTypNummer")]
        public virtual ArtikelTyp artikelTyp { get; set; }



        public string DisplayText => $"{LogiNummer} | {LogiKapacitet} pers | {Storlek} kvm | {Faciliteter} | {AntalRum} rum";
       
        [NotMapped]
        public string? TypDisplayText { get; set; }
    }
}
