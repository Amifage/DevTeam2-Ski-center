using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public virtual Skidlektion skidlektion { get; set; }

        public int? BokningsNummer { get; set; }
        public virtual Bokning bokning { get; set; }

    }
}
