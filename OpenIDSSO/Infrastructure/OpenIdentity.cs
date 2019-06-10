using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIDSSO.Infrastructure
{
    using Models;
    [Serializable]
    public class OpenIDIdentity:IOpenIDIdentity
    {
        private readonly UserModel _user;
        public OpenIDIdentity() : this(null) { }
        public OpenIDIdentity(UserModel user)
        {
            _user = user;
        }
        #region IIdentity 接口
        public string AuthenticationType
        {
            get { return "Custom"; }
        }

        public string Name
        {
            get { return _user != null ? _user.UserName : ""; }
        }

        public bool IsAuthenticated
        {
            get { return _user != null; }
        }

        #endregion

        #region 自定义属性

        public string Email { get { return _user!=null?_user.Email:""; } }

        public char Gender { get { return _user != null ? _user.Gender : ' '; } }
        /// <summary>
        /// 用户是否通过审核
        /// </summary>
        public bool IsValid { get { return true; } }

        #endregion
    }
}