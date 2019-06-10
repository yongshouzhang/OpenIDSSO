using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
namespace OpenIDSSO.Common
{
    public static class MD5Helper
    {
        public static string ComputeMD5(string msg)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(msg)))
                .Replace("-", "").ToLower();
        }
    }
}