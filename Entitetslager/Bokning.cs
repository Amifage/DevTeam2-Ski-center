using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    internal class Bokning

    {
        public int BokningsNummer { get; set; }
    // public Kund Kund { get; set; } 
    public DateTime BokningsDatum { get; set; }
    public string Status { get; set; }
    public DateTime SenastUppdaterad { get; set; }

    //   public List<KonferansRad>? KonferansRader { get; set; }
    //  public List<LogiRad>? LogiRader { get; set; }
    // public List<UtrustningRad>? UtrustningRader { get; set; }
    public List<SkidlektionRad>? SkidlektionRader { get; set; }

    }
}
