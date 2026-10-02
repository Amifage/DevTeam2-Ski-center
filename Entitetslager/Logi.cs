namespace Entitetslager
{
    public class Logi
    {
        public string LogiNummer { get; set; }
        public int LogiKapacitet { get; set; }
        public string Status { get; set; }
        public DateTime SenastUppdaterad { get; set; }
        public int AntalRum { get; set; }
        public int Storlek { get; set; }
        public string Facilitet { get; set; }

        public int? ArtikelTypNummer { get; set; }
        public virtual ArtikelTyp artikelTyp { get; set; }
    }
}
