using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class Personal
    {
        [Key] public int AnställningsNummer { get; set; }
        public string Roll { get; set; } 
        public string Förnamn { get; set; }
        public string Efternamn { get; set; } 
        public string Epost { get; set; } 
        public string Lösenord { get; set; }
        public DateTime SenastUppdaterad { get; set; }
    }
}
