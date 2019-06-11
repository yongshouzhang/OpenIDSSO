using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
namespace OpenIDSSO.Common
{
    using Infrastructure;
    public static class RpLogOffHelper
    {
        /// <summary>
        /// 退出所有rp站点
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="force">是否是强制下线</param>
        public static void LogOffAllRp(string user,bool force)
        {
            Dictionary<string, string> dic = RedisHelper.GetAllLoginSession(user + ":session");
            string root = Util.GetRootedUri("").ToString(),
                id = dic.TryGetValue(root, out id) ? id : "";
            if (!string.IsNullOrEmpty(id))
            {
                if (force)
                {
                    object lockId = new object();
                    var session = SessionHelper.GetSession(id, out lockId);
                    session.Items["customidentity"] = new OpenIDPrincipal(new OpenIDIdentity(null), true);
                    SessionHelper.SetSessionData(id, session, lockId);
                }else
                {
                    SessionHelper.RemoveSession(id);
                }
            }
            var list = dic.Where(obj=>!string.Equals(obj.Key,Util.GetRootedUri("").ToString(),StringComparison.Ordinal))
                .Select(obj => new { url = new Uri(obj.Key), session = obj.Value }).ToList();

            using (HttpClient client = new HttpClient())
            {
                list.ForEach(obj =>
                {
                    HttpContent content = new FormUrlEncodedContent(new[] {
                        new {name="session",value=obj.session},
                        new { name="force",value=force?"y":"n" }
                    }.ToDictionary(t => t.name, t => t.value));
                    var result = client.PostAsync(new Uri(obj.url, "account/oplogoff"), content).Result;
                    var retResult = result.Content.ReadAsStringAsync().Result;
                });
            }
        }
    }
}