using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIDRP.Models
{
    [Serializable]
    public class UserModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public char Gender { get; set; }
    }
}