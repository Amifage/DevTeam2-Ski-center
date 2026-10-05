using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Entitetslager;
using Microsoft.EntityFrameworkCore;

namespace Datalager
{
    public class GenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        public GenericRepository(DbSet<T> dbSet)
        {
            _dbSet = dbSet;
        }


        #region Generella metoder

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        #endregion
    }
}
