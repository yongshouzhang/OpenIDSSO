using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Security.Principal;

namespace OpenIDRP.Infrastructure
{
    using Models;
    using Common;
    public class FormsAuthenticationService : IFormsAuthentication
    {
        public string SignedInUsername
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

        public DateTime? SignedInTimestampUtc
        {
            get
            {
                //var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                //if (cookie != null)
                //{
                //    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                //    return ticket.IssueDate.ToUniversalTime();
                //}
                //else
                //{
                    return null;
                //}
            }
        }

        public void SignIn(UserModel userModel)
        {
            IPrincipal user = new OpenIDPrincipal(new OpenIDIdentity(userModel));
            HttpContext.Current.User = user;
            HttpContext.Current.Session["customidentity"] = user;
        }
        /// <summary>
        /// 是否强制（挤掉当登录用户)
        /// </summary>
        /// <param name="force"></param>
        public void SignOut(string username, bool force)
        {
            IPrincipal user = new OpenIDPrincipal(new OpenIDIdentity(null), force);
            HttpContext.Current.User = user;
            HttpContext.Current.Session["customidentity"] = user;
        }
    }
}
