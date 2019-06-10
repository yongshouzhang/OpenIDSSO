using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
namespace OpenIDSSO.Infrastructure
{
     public interface IOpenIDPrincipal:IPrincipal
    {
        bool IsForceLogOff { get; }
    }
}
