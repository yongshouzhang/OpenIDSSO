using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.SessionState;
using System.Threading;
using System.Collections.Concurrent;
namespace OpenIDSSO.Common
{
    public static class SessionHelper
    {
        #region  ConcurrentDictionary
        private static object _lock = new object();
        #endregion
        #region SessionStateStoreProviderBase
        private static bool _initializeStore;
        private static SessionStateStoreProviderBase _store;
        private static object _lockStore = new object();
        private static SessionStateStoreProviderBase SessionStore
        {
            get
            {
                return LazyInitializer.EnsureInitialized<SessionStateStoreProviderBase>(ref _store, ref _initializeStore, ref _lockStore, () =>
                {
                    IHttpModule module = HttpContext.Current.ApplicationInstance.Modules["Session"];
                    return module.GetType()
                      .GetField("_store", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(module) as SessionStateStoreProviderBase;
                });
            }
        }
        #endregion

        public static void SetSessionData(string id, SessionStateStoreData session, object lockId)
        {
            if (session != null)
            {
                SessionStore.SetAndReleaseItemExclusive(HttpContext.Current, id, session, lockId, false);
            }
        }

        public static SessionStateStoreData GetSession(string id, out object lockId)
        {
            lockId = new object();
            if (string.IsNullOrEmpty(id)) return null;
            if (SessionStore != null)
            {
                bool isLock = false;
                TimeSpan lockAge = TimeSpan.Zero;
                SessionStateActions state = SessionStateActions.InitializeItem;
                SessionStateStoreData data = null;
                try
                {
                    data = SessionStore.GetItemExclusive(HttpContext.Current, id, out isLock, out lockAge, out lockId, out state);
                }
                catch { }
                finally
                {
                    if (data != null)
                    {
                        SessionStore.ReleaseItemExclusive(HttpContext.Current, id, lockId);
                    }
                }
                return data;
            }
            return null;

        }

        public static SessionStateStoreData GetSession(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (SessionStore != null)
            {
                bool isLock = false;
                object lockId = new object();
                TimeSpan lockAge = TimeSpan.Zero;
                SessionStateActions state = SessionStateActions.InitializeItem;
                SessionStateStoreData data = null;
                try
                {
                    data = SessionStore.GetItemExclusive(HttpContext.Current, id, out isLock, out lockAge, out lockId, out state);
                }
                catch { }
                finally
                {
                    if (data != null)
                    {
                        SessionStore.ReleaseItemExclusive(HttpContext.Current, id, lockId);
                    }
                }
                return data;

            }
            return null;
        }
        public static void RemoveSession(string id)
        {
            if (SessionStore != null)
            {
                SessionStore.RemoveItem(HttpContext.Current, id, 0, null);
            }
        }
    }
}