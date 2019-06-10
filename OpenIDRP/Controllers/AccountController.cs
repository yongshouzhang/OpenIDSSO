using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using System.Threading;
using System.Threading.Tasks;
namespace OpenIDRP.Controllers
{
    using Common;
    using Models;
    using Infrastructure;
    public class AccountController : Controller
    {
        private static OpenIdRelyingParty RP = new OpenIdRelyingParty();
        private static readonly string EndPoint = "http://192.168.0.100:8800/";
        private readonly IFormsAuthentication _formsAuth;
        public AccountController(IFormsAuthentication formsAuth)
        {
            this._formsAuth = formsAuth;
        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [ValidateInput(false)]
        public async Task<ActionResult> LogOn()
        {
            var response = await RP.GetResponseAsync(this.Request, this.Response.ClientDisconnectedToken);

            if (response == null)
            {
                var request = await RP.CreateRequestAsync(EndPoint);
                StoreRequest store = new StoreRequest();
                store.Attributes.Add(new AttributeValues(Util.GetRootedUri("").ToString(), new[] { Session.SessionID }));
                request.AddExtension(store);
                var redirecting = await request.GetRedirectingResponseAsync(this.Response.ClientDisconnectedToken);
                //设置本地响应头部信息
                Response.ContentType = redirecting.Content.Headers.ContentType.ToString();
                return redirecting.AsActionResult();
            }

            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:

                    var fetch = response.GetExtension<FetchResponse>();

                    var dic = fetch.Attributes.Where(obj =>
                      {
                          return obj.TypeUri.IndexOf(EndPoint) == 0;
                      }).Select(obj => new { name = obj.TypeUri.Replace(EndPoint, ""), value = obj.Values[0] })
                     .ToLookup(obj => obj.name).ToDictionary(obj => obj.Key, obj => obj.First().value);

                    UserModel user = null;
                    if (!dic.Any(obj => string.IsNullOrEmpty(obj.Value)))
                    {
                        user = new UserModel
                        {
                            UserName = dic["username"],
                            Email = dic["email"],
                            Gender = dic["gender"][0]
                        };
                    }
                    this._formsAuth.SignIn(user);
                    return this.RedirectToAction("index", "home");
                case AuthenticationStatus.Canceled:
                    break;
                case AuthenticationStatus.Failed:
                    break;
            }

            return new EmptyResult();
        }

        public ActionResult LogOff()
        {
            Session["customidentity"] = null;
            HttpContext.User = new OpenIDPrincipal(new OpenIDIdentity(null));
            return this.Redirect(EndPoint + "account/logoff?returnUrl=" + Util.GetRootedUri("home/index").ToString());
        }


        [HttpPost]
        /// <summary>
        /// 响应OP,另外添加自定属性，限制请求IP 
        /// </summary>
        /// <returns></returns>
        public JsonResult OpLogOff(string session,string force)
        {
            //如果是强制被下线,须给前台提示
            if (force.ToLower()=="y")
            {
                var data = SessionHelper.GetSession(session);
                if (data != null)
                {
                    data.Items["customidentity"] = new OpenIDPrincipal(new OpenIDIdentity(null), true);
                }
            }
            else
            {
                //SessionHelper.RemoveSession(session);
            }

                SessionHelper.RemoveSession(session);
            return Json(new { result = true });
        }

    }
}