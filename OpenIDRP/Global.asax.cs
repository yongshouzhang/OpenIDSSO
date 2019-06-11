using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Security.Principal;
namespace OpenIDRP
{
    using Infrastructure;
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
            Session["customidentity"] = user;
            HttpContext.Current.User = user;
        }
    }
}
