using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Provider;

namespace OpenIDSSO.Infrastructure
{
    public  static class Util
    {
        public  static Uri GetRootedUri(string value)
        {
            string appPath = HttpContext.Current.Request.ApplicationPath.ToLowerInvariant();
            if (!appPath.EndsWith("/"))
            {
                appPath += "/";
            }
            return new Uri(HttpContext.Current.Request.Url, appPath + value);
        }

        public static Uri GetUserClaimIdentifier(string username)
        {
            return GetRootedUri("user/" + username);
        }

        public static string GetRequestHost(Realm url)
        {
            return url.Scheme + "://" + url.Host + (url.Port == 80 ? "" : (":" + url.Port.ToString())) + "/";
        }
        public static string GetRpStoreExtension(string key)
        {
            if (ProviderEndpoint.PendingRequest == null) return null;
            StoreRequest store = ProviderEndpoint.PendingRequest.GetExtension<StoreRequest>();
            if (store == null || store.Attributes.Count == 0) return null;
            return store.Attributes.Where(obj => string.Equals(obj.TypeUri, key, StringComparison.Ordinal))
                .Select(obj => obj.Values.Count > 0 ? obj.Values[0] : null)
                .FirstOrDefault();
        }

    }
}