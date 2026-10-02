using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class Kund
    {
        public int KundNummer { get; set; }
        public string Epost { get; set; } = null!;
        public string Telefonnummer { get; set; } = null!;
        public string Adress { get; set; } = null!;
        public string Postnummer { get; set; } = null!;
        public string Ort { get; set; } = null!;
        public DateTime SenastUppdaterad { get; set; }
    }
}
