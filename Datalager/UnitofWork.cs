using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entitetslager;
using Microsoft.EntityFrameworkCore;

namespace Datalager
{
    public class UnitofWork : IDisposable
    {
        private readonly SkiContext _skiContext;

        public GenericRepository<Personal> PersonalRepository { private set; get; }


        public GenericRepository<Kund> KundRepository { get; private set; }
        public GenericRepository<PrivatKund> PrivatKundRepository { get; private set; }
        public GenericRepository<FöretagsKund> FöretagsKundRepository { get; private set; }


        public GenericRepository<Utrustning> UtrustningRepository { private set; get; }
        public GenericRepository<UtrustningRad> UtrustningRadRepository { get; private set; }
        public GenericRepository<UtrustningPaket> UtrustningPaketRepository { get; private set; }
        public GenericRepository<UtrustningPaketInnehåll> UtrustningPaketInnehållRepository { get; private set; }
        public GenericRepository<UtrustningPaketRad> UtrustningPaketRadRepository { get; private set; }



        public GenericRepository<Konferens> KonferensRepository { private set; get; }
        public GenericRepository<KonferensRad> KonferensRadRepository { get; private set; }


        public GenericRepository<Logi> LogiRepository { private set; get; }
        public GenericRepository<LogiRad> LogiRadRepository { get; private set; }


        public GenericRepository<Skidlektion> SkidlektionRepository { private set; get; }
        public GenericRepository<SkidlektionRad> SkidlektionRadRepository { get; private set; }


        public GenericRepository<ArtikelTyp> ArtikelTypRepository { get; private set; }
        public GenericRepository<Pris> PrisRepository { get; private set; }
        public GenericRepository<PrisRegel> PrisRegelRepository { get; private set; }


        public GenericRepository<Bokning> BokningRepository { private set; get; }
        public GenericRepository<Faktura> FakturaRepository { get; private set; }

        public UnitofWork(SkiContext dbContext)
        {
            _skiContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));


            PersonalRepository = new GenericRepository<Personal>(_skiContext.Personal);

            KundRepository = new GenericRepository<Kund>(_skiContext.Kund);
            PrivatKundRepository = new GenericRepository<PrivatKund>(_skiContext.PrivatKund);
            FöretagsKundRepository = new GenericRepository<FöretagsKund>(_skiContext.FöretagsKund);

            UtrustningRepository = new GenericRepository<Utrustning>(_skiContext.Utrustning);
            UtrustningRadRepository = new GenericRepository<UtrustningRad>(_skiContext.UtrustningRad);
            UtrustningPaketRepository = new GenericRepository<UtrustningPaket>(_skiContext.UtrustningPaket);
            UtrustningPaketInnehållRepository = new GenericRepository<UtrustningPaketInnehåll>(_skiContext.UtrustningPaketInnehåll);
            UtrustningPaketRadRepository = new GenericRepository<UtrustningPaketRad>(_skiContext.UtrustningPaketRad);

            KonferensRepository = new GenericRepository<Konferens>(_skiContext.Konferens);
            KonferensRadRepository = new GenericRepository<KonferensRad>(_skiContext.KonferensRad);

            LogiRepository = new GenericRepository<Logi>(_skiContext.Logi);
            LogiRadRepository = new GenericRepository<LogiRad>(_skiContext.LogiRad);

            SkidlektionRepository = new GenericRepository<Skidlektion>(_skiContext.Skidlektion);
            SkidlektionRadRepository = new GenericRepository<SkidlektionRad>(_skiContext.SkidlektionRad);

            ArtikelTypRepository = new GenericRepository<ArtikelTyp>(_skiContext.ArtikelTyp);
            PrisRepository = new GenericRepository<Pris>(_skiContext.Pris);
            PrisRegelRepository = new GenericRepository<PrisRegel>(_skiContext.PrisRegel);

            BokningRepository = new GenericRepository<Bokning>(_skiContext.Bokning);
            FakturaRepository = new GenericRepository<Faktura>(_skiContext.Faktura);
        }

        public int Save()
        {
            return _skiContext.SaveChanges();
        }
        public void Dispose()
        {
            _skiContext?.Dispose();
        }
    }
}
