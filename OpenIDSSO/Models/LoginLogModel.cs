using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIDSSO.Models
{
    public class LoginLogModel
    {
        public Guid ID { get; set; }
        public string UserName { get; set; }

        public string OpenIdClaimedIdentifier { get; set; }

        public string OpenIdFriendlyIdentifier { get; set; }

        public DateTime AddTime { get; set; }

        public int Flag { get; set; }
    }
}