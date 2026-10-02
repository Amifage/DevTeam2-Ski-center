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
        public GenericRepository<Utrustning> UtrustningRepository { private set; get; }
        public GenericRepository<Konferens> KonferensRepository { private set; get; }
        public GenericRepository<Logi> LogiRepository { private set; get; }
        public GenericRepository<Skidlektion> SkidlektionRepository { private set; get; }
        public GenericRepository<Bokning> BokningRepository { private set; get; }

        public UnitOfWork(SkiContext dbContext)
        {
            dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));


            PersonalRepository = new GenericRepository<Personal>(dbContext.Personal);
            KundRepository = new GenericRepository<Kund>(dbContext.Kund);
            UtrustningRepository = new GenericRepository<Utrustning>(dbContext.Utrustning);
            KonferensRepository = new GenericRepository<Konferens>(dbContext.Konferens);
            LogiRepository = new GenericRepository<Logi>(dbContext.Logi);
            SkidlektionRepository = new GenericRepository<Skidlektion>(dbContext.Skidlektion);
            BokningRepository = new GenericRepository<Bokning>(dbContext.Bokning);

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
