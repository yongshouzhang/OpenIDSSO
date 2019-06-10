using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace OpenIDSSO.Infrastructure
{
    using Models;
    public interface IFormsAuthentication
    {
        string SignedInUsername { get; }

        DateTime? SignedInTimestampUtc { get; }

        void SignIn(UserModel user, bool createPersistentCookie);

        void SignOut(string username,bool force);
    }
}
