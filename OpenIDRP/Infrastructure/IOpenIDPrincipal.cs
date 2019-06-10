using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
namespace OpenIDRP.Infrastructure
{
     public interface IOpenIDPrincipal:IPrincipal
    {
        bool IsForceLogOff { get; }
    }
}
