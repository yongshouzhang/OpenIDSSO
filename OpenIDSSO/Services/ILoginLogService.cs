using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
namespace OpenIDSSO.Services
{
    using Models;
    public interface ILoginLogService
    {
        bool Add(LoginLogModel model);

        bool Delete(LoginLogModel model);

        IQueryable<LoginLogModel> GetListByPage<TKey>(int pageIndex, int pageSize, Expression<Func<LoginLogModel, bool>> predicate, Expression<Func<LoginLogModel, TKey>> orderBy, out int count);

    }
}