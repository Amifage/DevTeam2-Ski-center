using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class KonferensRad
    {
        [Key] public int KonferensRadNummer { get; set; }
        public DateTime SenastUppdaterad { get; set; }
        public DateTime StartTid { get; set; }
        public DateTime SlutTid { get; set; }
        public decimal KonferensBelopp { get; set; }
        public int AntalPersoner { get; set; }


        public string? KonferensNummer { get; set; }
        [ForeignKey(nameof(KonferensNummer))] // Kopplar KonferensNummer till konferens, samma som i LogiRad
        public virtual Konferens konferens { get; set; } //nav prop

        public int? BokningsNummer { get; set; }
        [ForeignKey(nameof(BokningsNummer))] // Kopplar BokningsNummer till bokning, samma som i LogiRad
        public virtual Bokning bokning { get; set; }//nav prop
    }
}
