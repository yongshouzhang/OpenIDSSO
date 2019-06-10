using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;

namespace OpenIDRP.Infrastructure
{
    [Serializable]
    public class OpenIDPrincipal:IOpenIDPrincipal
    {
        private readonly IOpenIDIdentity _identity;
        private readonly bool _forceLogOff;
        public OpenIDPrincipal() : this(null) { }
        public OpenIDPrincipal(IOpenIDIdentity identity) : this(identity,false) { }
        public OpenIDPrincipal(IOpenIDIdentity identity,bool forceLogOff)
        {
            _identity = identity;
            _forceLogOff = forceLogOff;
        }
        public IIdentity Identity
        {
            get { return this._identity; }
        }
        public bool IsInRole(string role)
        {
            // TODO 角色判断
            return false;
        }
        /// <summary>
        /// 是否被强制下线
        /// </summary>
        public bool IsForceLogOff { get { return _forceLogOff; } }
    }
}