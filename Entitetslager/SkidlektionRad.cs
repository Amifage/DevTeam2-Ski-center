using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entitetslager
{
    public class SkidlektionRad
    {
        [Key] public int SkidLektionRadNummer { get; set; }
        public DateTime Datum { get; set; }
        public decimal SkidlektionBelopp { get; set; }
        public int AntalPersoner { get; set; }       
        public DateTime SenastUppdaterad { get; set; }


        public int? SkidlektionNummer { get; set; }
        [ForeignKey(nameof(SkidlektionNummer))] // Talar om för EF att SkidlektionNummer är kopplingen till skidlektion, annars skapas en extra kolumn (SkidlektionNummer1)
        public virtual Skidlektion skidlektion { get; set; }

        public int? BokningsNummer { get; set; }
        [ForeignKey(nameof(BokningsNummer))] // Talar om för EF att BokningsNummer är kopplingen till bokning, annars skapas en extra kolumn (BokningsNummer1)
        public virtual Bokning bokning { get; set; }

    }
}
