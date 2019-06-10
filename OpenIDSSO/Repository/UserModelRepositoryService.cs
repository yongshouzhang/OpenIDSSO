using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
namespace OpenIDSSO.Repository
{
    using Models;
    using Services;
    public class UserModelRepositoryService:IUserModelService
    {
        private readonly IBaseRepository<UserModel> _repository;
        public UserModelRepositoryService(IBaseRepository<UserModel> repository )
        {
            _repository = repository;
        }

        public  bool Add(UserModel model)
        {
            return _repository.Add(model);
        }

        public  bool Delete(UserModel model)
        {
            return _repository.Delete(model);
        }

        public  bool Update(UserModel model)
        {
            return _repository.Update(model);
        }

        public bool Exists(UserModel model)
        {
            return _repository.Count(t => t.Email == model.Email) > 0;
        }

        public UserModel Get(Expression<Func<UserModel,bool>> user)
        {
            return _repository.Get(user);
        }

        public IQueryable<UserModel> GetList(Expression<Func<UserModel,bool>> user)
        {
            return _repository.GetList(user);
        }
    }
}