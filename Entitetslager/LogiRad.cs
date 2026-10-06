using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitetslager
{
    public class LogiRad
    {
        [Key] public int LogiRadNummer { get; set; }      
        public DateTime StartDatum { get; set; }
        public DateTime SlutDatum { get; set; }
        public decimal LogiBelopp { get; set; }
        public int AntalPersoner { get; set; }
        public DateTime SenastUppdaterad { get; set; }

        public string? LogiNummer {  get; set; }
        [ForeignKey(nameof(LogiNummer))]
        public virtual Logi logi { get; set; }


        public int? BokningsNummer { get; set; }
        [ForeignKey(nameof(BokningsNummer))]
        public virtual Bokning bokning { get; set; } // nav prop


        [NotMapped]
        public string? LogiDisplayText { get; set; } //Chats påhitt, denna gör så vi kan använda display text i Logi i Bokning /Sara

    }
}
