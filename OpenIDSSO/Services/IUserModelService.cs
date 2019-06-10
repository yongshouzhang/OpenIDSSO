using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
namespace OpenIDSSO.Services
{
    using Models;
    public interface IUserModelService
    {
        UserModel Get(Expression<Func<UserModel, bool>> predicate);

        bool Add(UserModel model);

        bool Delete(UserModel model);

        bool Update(UserModel model);

        bool Exists(UserModel model);
    }
}