using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using iTimeService.Entities;

using System.Data.Entity;

namespace iTimeService.Concrete
{
    public class Repository<T> : IRepository<T> where T:class
    {
        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public Repository(DbContext _dbContext)
        {
            if (_dbContext == null)
                throw new ArgumentNullException("Null DbContext");
            DbContext = _dbContext;
            DbSet = DbContext.Set<T>();
        }
        public virtual IQueryable<T> All()
        {
            return DbSet;
        }
        public virtual T Find(int id)
        {
            return DbSet.Find(id);
        }
        public virtual T Find(string searchStr)
        {
            return DbSet.Find(searchStr);
        }
        public virtual void Insert(T entity)
        {
            if (DbContext.Entry(entity).State != EntityState.Detached)
            {
                DbContext.Entry(entity).State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }
        public virtual void Update(T entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbContext.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            if (DbContext.Entry(entity).State != EntityState.Deleted)
            {
                DbContext.Entry(entity).State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }
        public virtual void Delete(int id)
        {
            var entity = Find(id);
            if (entity == null) return; //assume already deleted
            Delete(entity);
        }
        public void Save()
        {
            DbContext.SaveChanges();
        }
    }
}
