using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Linq.Expressions;
namespace OpenIDSSO.Repository
{
    public  class BaseRepository<T>:IBaseRepository<T>  where T :class ,new() 
    {
        public OpenIdDbContext _db
        {
            get { return OpenIdDbContext.Instance; }
        }

        public virtual bool Add(T model)
        {
            _db.Entry<T>(model).State = EntityState.Added;
            return _db.SaveChanges() == 1;
        }

        public virtual int Add(IEnumerable<T> models)
        {
            _db.Set<T>().AddRange(models);
            return _db.SaveChanges();
        }

        public virtual bool Update(T model)
        {
            _db.Entry<T>(model).State = EntityState.Modified;
            return _db.SaveChanges() == 1;
        }

        public virtual int Update(IEnumerable<T> models)
        {
            List<T> tmp = models.ToList();
            tmp.ForEach(model =>
            {
                _db.Entry<T>(model).State = EntityState.Modified;
            });
            return _db.SaveChanges();
        }

        public virtual bool Delete(T model)
        {
            _db.Entry<T>(model).State = EntityState.Deleted;
            return _db.SaveChanges() == 1;
        }

        public virtual  int Delete(Expression<Func<T,bool>> predicate)
        {
            _db.Set<T>().Where(predicate).ToList().ForEach(obj =>
            {
                _db.Entry<T>(obj).State = EntityState.Deleted;
            });
            return _db.SaveChanges();
        }

        public virtual int Count(Expression<Func<T,bool>> predicate)
        {
            return _db.Set<T>().Count(predicate);
        }
        public virtual T Get(Expression<Func<T,bool>> predicate)
        {
            return _db.Set<T>().FirstOrDefault(predicate);
        }

        public virtual IQueryable<T> GetList(Expression<Func<T,bool>> predicate)
        {
            return _db.Set<T>().Where(predicate);
        }

        public virtual IQueryable<T> GetListByPage<TKey>(int start,int count,Expression<Func<T,bool>> predicate,Expression<Func<T,TKey>> orderBy,out int total)
        {
            total= _db.Set<T>().Count(predicate);
            return _db.Set<T>().Where(predicate).OrderBy(orderBy)
                .Skip(start).Take(count);
        }
    }
}