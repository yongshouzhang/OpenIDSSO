using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIDRP.Common
{
    public static class Util
    {
        public static Uri GetRootedUri(string value)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath.ToLowerInvariant();
            if (!appPath.EndsWith("/"))
            {
                appPath += "/";
            }
            return new Uri(HttpContext.Current.Request.Url, appPath + value);
        }

    }
}