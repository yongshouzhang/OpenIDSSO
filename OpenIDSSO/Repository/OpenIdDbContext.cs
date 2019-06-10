using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Threading;

namespace OpenIDSSO.Repository
{
    using Models;
    public class OpenIdDbContext:DbContext
    {
        private static OpenIdDbContext _db = null;
        private static bool _ready=false;
        private static object _lock = new object(); 

        private OpenIdDbContext():base("openid")
        {
        }

        public static OpenIdDbContext Instance
        {
            get
            {
                return _ready ? _db : LazyInitializer.EnsureInitialized<OpenIdDbContext>(ref _db, ref _ready, ref _lock, () => new OpenIdDbContext());
            }
        }
        public DbSet<UserModel> UserModel { get; set; }

        public DbSet<LoginLogModel> LoginLogModel { get; set; }
    }
}