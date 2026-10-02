using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class Skidlektion
    {
        public int SkidLektionsNummer { get; set; }
        public string SkidLektionsTyp { get; set; } = null!;
        public Personal Personal { get; set; } = null!;
        public int Kapacitet { get; set; }
        public DateTime SenastUppdaterad { get; set; }

    }
}
