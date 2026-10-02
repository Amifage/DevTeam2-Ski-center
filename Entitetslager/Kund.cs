using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Kund
    {
        public int KundNummer { get; set; }
        public string Epost { get; set; } 
        public string Telefonnummer { get; set; } 
        public string Adress { get; set; }
        public string Postnummer { get; set; } 
        public string Ort { get; set; } 
        public DateTime SenastUppdaterad { get; set; }
    }
}
