using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace OpenIDSSO.Repository
{
    public interface IBaseRepository<T> where T:  class ,new()
    {
        bool Add(T model);
        int Add(IEnumerable<T> models);

        int Count(Expression<Func<T, bool>> predicate);
        T Get(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetList(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetListByPage<TKey>(int start, int count, Expression<Func<T, bool>> predication,
            Expression<Func<T, TKey>> orderBy, out int total);
        bool Update(T model);
        int Update(IEnumerable<T> models);
        bool Delete(T model);
        int Delete(Expression<Func<T, bool>> predicate);
    }
}