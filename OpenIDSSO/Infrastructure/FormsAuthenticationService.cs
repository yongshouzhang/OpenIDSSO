using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DotNetOpenAuth.OpenId.Provider;

namespace OpenIDSSO.Infrastructure
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
                var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    var ticket = FormsAuthentication.Decrypt(cookie.Value);
                    return ticket.IssueDate.ToUniversalTime();
                }
                else
                {
                    return null;
                }
            }
        }

        public void SignIn(UserModel user, bool createPersistentCookie)
        {
            RedisHelper.SetLoginSession(user.UserName + ":session",
                Util.GetRootedUri("").ToString(), HttpContext.Current.Session.SessionID);
            IHostProcessedRequest RpRequest = ProviderEndpoint.PendingRequest;
            //如果从其他站点登录
            if (RpRequest != null)
            {
                string url = Util.GetRequestHost(RpRequest.Realm);
                string rpSession = Util.GetRpStoreExtension(url);
                if (!string.IsNullOrEmpty(rpSession))
                {
                    // 此处原是接受 RP 发来的SessionId，后发现SessionID会发生变化，和OP的一样
                    RedisHelper.SetLoginSession(user.UserName + ":session", url, rpSession);
                }
            }

            RedisHelper.SetUserInfo(user.UserName + ":master", user);

            if (!createPersistentCookie)
            {
                int span = (int)TimeSpan.FromMinutes(10).TotalSeconds;
                RedisHelper.SetExpire(user.UserName + ":master", span);
                RedisHelper.SetExpire(user.UserName + ":session", span);
            }

            OpenIDPrincipal identity = new OpenIDPrincipal(new OpenIDIdentity(user));
            HttpContext.Current.Session["customidentity"] = identity;
            HttpContext.Current.User = identity;
           
        }
        /// <summary>
        /// 是否强制（挤掉当登录用户)
        /// </summary>
        /// <param name="force"></param>
        public void SignOut(string username, bool force)
        {
            RpLogOffHelper.LogOffAllRp(username, force);

            RedisHelper.ClearLoginSession(username + ":session");
            RedisHelper.ClearUserInfo(username + ":master");
            RedisHelper.ClearTempSession(username + ":tempsession");
            OpenIDPrincipal identity = new OpenIDPrincipal(new OpenIDIdentity(null), force);
            HttpContext.Current.User = identity;
            HttpContext.Current.Session["customidentity"] = identity;
        }
    }
}
