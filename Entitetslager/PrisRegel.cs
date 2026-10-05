using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class PrisRegel
    {
        [Key] public int PrisRegelNummer { get; set; }
        public string RegelKod { get; set; }
        public string RegelBeskrivning { get; set; }
    }
}
