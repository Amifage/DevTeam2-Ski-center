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
        public DbSet<Utrustning> Utrustning { get; set; }
        public DbSet<Konferens> Konferens { get; set; }
        public DbSet<Logi> Logi { get; set; }
        public DbSet<Skidlektion> Skidlektion { get; set; }
        public DbSet<Bokning> Bokning { get; set; }

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
