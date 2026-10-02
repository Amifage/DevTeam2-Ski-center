using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Entitetslager;

namespace Datalager
{
    public class SkiContext : DbContext
    {
        public DbSet<Personal> Personal { get; set; }

        public DbSet<Kund> Kund { get; set; }
        public DbSet<PrivatKund> PrivatKund { get; set; }
        public DbSet<FöretagsKund> FöretagsKund { get; set; }


        public DbSet<Utrustning> Utrustning { get; set; }
        public DbSet<UtrustningRad> UtrustningRad { get; set; }
        public DbSet<UtrustningPaket> UtrustningPaket { get; set; }
        public DbSet<UtrustningPaketInnehåll> UtrustningPaketInnehåll { get; set; }
        public DbSet<UtrustningPaketRad> UtrustningPaketRad { get; set; }

        public DbSet<Konferens> Konferens { get; set; }
        public DbSet<KonferensRad> KonferensRad { get; set; }

        public DbSet<Logi> Logi { get; set; }
        public DbSet<LogiRad> LogiRad { get; set; }


        public DbSet<Skidlektion> Skidlektion { get; set; }
        public DbSet<SkidlektionRad> SkidlektionRad { get; set; }


        public DbSet<ArtikelTyp> ArtikelTyp { get; set; }
        public DbSet<Pris> Pris { get; set; }
        public DbSet<PrisRegel> PrisRegel { get; set; }


        public DbSet<Bokning> Bokning { get; set; }
        public DbSet<Faktura> Faktura { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(
                "Server=sqlutb4-db.hb.se,56077;" +
                "Database=suht2602;" +
                "User Id=suht2602;" + //Inloggningsuppgifter
                "Password=QJA641;" +
                "Encrypt=True;" +
                "TrustServerCertificate=True;");
        }
    }
}
