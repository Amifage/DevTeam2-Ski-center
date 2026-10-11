using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Konferens
    {
        [Key] public string KonferensNummer { get; set; }
        public int KonferensKapacitet { get; set; }
        public string Status { get; set; }
        public DateTime SenastUppdaterad { get; set; }

        public int? ArtikelTypNummer { get; set; }

        [ForeignKey(nameof(ArtikelTypNummer))]
        public virtual ArtikelTyp? artikelTyp { get; set; }


        [NotMapped]
        public string DisplayText => $"{KonferensNummer} | max {KonferensKapacitet} personer";
    }
}
