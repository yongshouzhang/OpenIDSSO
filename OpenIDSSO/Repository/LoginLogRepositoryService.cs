using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
namespace OpenIDSSO.Repository
{
    using Services;
    using Models;
    public class LoginLogRepositoryService:BaseRepository<LoginLogModel>,ILoginLogService
    {
        public new bool Add(LoginLogModel model)
        {
            return base.Add(model);
        }

        public new bool Delete(LoginLogModel model)
        {
            return base.Delete(model);
        }

        public new IQueryable<LoginLogModel> GetListByPage<TKey>(int pageIndex,int pageSize,Expression<Func<LoginLogModel,bool>>predicate, Expression<Func<LoginLogModel,TKey>>orderBy,out int count)
        {
            int start = (pageIndex - 1) * pageSize + 1, takeCount = pageSize;
            return base.GetListByPage<TKey>(start, takeCount, predicate, orderBy, out count);
        }
    }
}