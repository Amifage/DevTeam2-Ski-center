using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class SkidlektionRad
    {
        public int SkidLektionsRadNummer { get; set; }
        //public Bokning Bokning { get; set; } = null!;
        public DateTime Datum { get; set; }
        public decimal SkidlektionBelopp { get; set; }
        public int AntalPersoner { get; set; }
        public Skidlektion Skidlektion { get; set; } = null!;
        public DateTime SenastUppdaterad { get; set; }
    }
}
