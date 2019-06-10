using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
using System.Threading;

namespace OpenIDSSO
{
    using Infrastructure;
    using Common;
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AutofacConfig.RegisterComponent();
        }


        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            IPrincipal user = Session["customidentity"] as IOpenIDPrincipal ?? new OpenIDPrincipal(new OpenIDIdentity(null));
            if (user.Identity.IsAuthenticated)
            {
                var masterSession = RedisHelper.GetLoginSession(user.Identity.Name + ":session", Util.GetRootedUri("").ToString());
                if (masterSession != Session.SessionID)
                {
                    user = new OpenIDPrincipal(new OpenIDIdentity(null));
                }
            }
            HttpContext.Current.User = user;
        }

    }
}
