using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Behaviors;
using DotNetOpenAuth.OpenId.Extensions;
using DotNetOpenAuth.OpenId.Extensions.ProviderAuthenticationPolicy;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.Provider;
using DotNetOpenAuth.OpenId.Provider.Behaviors;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using System.Threading.Tasks;
using CSRedis;

namespace OpenIDSSO.Controllers
{
    using Infrastructure;
    using Common;
    using Models;
    using Services;
    public class OpenIdController : Controller
    {
        internal static OpenIdProvider OpenIdProvider = new OpenIdProvider();

        private readonly IFormsAuthentication _formsAuth;

        private readonly IUserModelService _userService;
    
        public OpenIdController(IFormsAuthentication formsAuthentication,IUserModelService userService)
        {
            this._formsAuth = formsAuthentication;
            this._userService = userService;
        }

        [ValidateInput(false)]
        public async Task<ActionResult> Provider()
        {
            IRequest request = await OpenIdProvider.GetRequestAsync(this.Request, this.Response.ClientDisconnectedToken);

            if (request == null)
            {
                return View();
            }
            if (request.IsResponseReady)
            {
                var response = await OpenIdProvider.PrepareResponseAsync(request, this.Response.ClientDisconnectedToken);
                Response.ContentType = response.Content.Headers.ContentType.ToString();
                return response.AsActionResult();
            }
            //缓存在cache 中
            ProviderEndpoint.PendingRequest = (IHostProcessedRequest)request;
            //查找pape 扩展信息
            var papeRequest = ProviderEndpoint.PendingRequest.GetExtension<PolicyRequest>();
            if (papeRequest != null && papeRequest.MaximumAuthenticationAge.HasValue)
            {
                TimeSpan timeSinceLogin = DateTime.UtcNow - (this._formsAuth.SignedInTimestampUtc ?? DateTime.UtcNow);
                if (timeSinceLogin > papeRequest.MaximumAuthenticationAge.Value)
                {
                    return this.RedirectToAction("LogOn", "Account", new { returnUrl = this.Url.Action("ProcessAuthRequest") });
                }
            }
            return await this.ProcessAuthRequest();
        }

        public async Task<ActionResult> ProcessAuthRequest()
        {
            if (ProviderEndpoint.PendingRequest == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            ActionResult response = await this.AutoRespondIfPossibleAsync();
            if (response != null)
            {
                return response;
            }

            if (ProviderEndpoint.PendingRequest.Immediate)
            {
                return await this.SendAssertion();
            }

            if (!ProviderEndpoint.PendingAuthenticationRequest.IsDirectedIdentity && !this.UserControlsIdentifier(ProviderEndpoint.PendingAuthenticationRequest))
            {
                return this.Redirect(this.Url.Action("LogOn", "Account", new { returnUrl = this.Request.Url }));
            }
            return this.RedirectToAction("SendResponse");
        }

        [Authorize]
        public async Task<ActionResult> SendResponse(bool confirm=true)
        {
            if (ProviderEndpoint.PendingRequest == null)
            {
                return this.RedirectToAction("Index", "Home");
            }
           
            ActionResult response = await this.AutoRespondIfPossibleAsync();
            if (response != null)
            {
                return response;
            }

            if (!ProviderEndpoint.PendingAuthenticationRequest.IsDirectedIdentity && !this.UserControlsIdentifier(ProviderEndpoint.PendingAuthenticationRequest))
            {
                return this.Redirect(this.Url.Action("LogOn", "Account", new { returnUrl = this.Request.Url }));
            }

            if (ProviderEndpoint.PendingAnonymousRequest != null)
            {
                ProviderEndpoint.PendingAnonymousRequest.IsApproved = confirm;
            }
            else if (ProviderEndpoint.PendingAuthenticationRequest != null)
            {
                ProviderEndpoint.PendingAuthenticationRequest.IsAuthenticated = confirm;
            }
            else
            {
                throw new InvalidOperationException("There's no pending authentication request!");
            }
            return await this.SendAssertion();
        }

        public ActionResult AskUser()
        {
            if (!(bool)(Session["_askuser_"] ?? false))
            {
                return this.RedirectToAction("index", "home");
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post),ValidateAntiForgeryToken]
        public async Task<ActionResult> AskUser(bool confirm=false)
        {

            bool flag = (bool)(Session["_askuser_"] ?? false),
                remember = (bool)(Session["_remember_"] ?? false);
            string username = (Session["_tempuser_"] ?? "").ToString();

            Session.Remove("_askuser_");
            Session.Remove("_remember_");
            Session.Remove("_tempuser_");

            if(!flag||string.IsNullOrEmpty(username))
            {
                return this.RedirectToAction("Index", "Home");
            }
            if (confirm)
            {
                UserModel user = RedisHelper.GetUserInfo(username) ?? _userService.Get(obj => obj.UserName == username);
                if (user == null)
                {
                    return this.RedirectToAction("Index", "Home");
                }

                //退出当前用户
                this._formsAuth.SignOut(username, true);

                //登录
                this._formsAuth.SignIn(user, remember);

            }
            return await this.SendResponse(confirm);
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<ActionResult> SendAssertion()
        {
            var pendingRequest = ProviderEndpoint.PendingRequest;
            var authReq = pendingRequest as IAuthenticationRequest;
            ProviderEndpoint.PendingRequest = null;
            if (pendingRequest == null)
            {
                throw new InvalidOperationException("There's no pending authentication request!");
            }

            if (authReq == null || !(authReq.IsAuthenticated??false))
            {
                authReq.IsAuthenticated = false;
            }

            if (authReq != null && authReq.IsAuthenticated.Value)
            {
                if (authReq.IsDirectedIdentity)
                {
                    authReq.LocalIdentifier = Util.GetUserClaimIdentifier(User.Identity.Name);
                }

                if (!authReq.IsDelegatedIdentifier)
                {
                    authReq.ClaimedIdentifier = authReq.LocalIdentifier;
                }

                // TODO 发送附加信息
                var fetchResp = new FetchResponse();

                UserModel user = RedisHelper.GetUserInfo(User.Identity.Name + ":master");
                fetchResp.Attributes.AddRange(new AttributeValues[]
                {
                    new AttributeValues(Util.GetRootedUri("username").ToString(),user.UserName),
                    new AttributeValues(Util.GetRootedUri("email").ToString(),user.Email),
                    new AttributeValues(Util.GetRootedUri("gender").ToString(),user.Gender.ToString())
                });
                pendingRequest.AddResponseExtension(fetchResp);
            }

            var response = await OpenIdProvider.PrepareResponseAsync(pendingRequest, this.Response.ClientDisconnectedToken);
            Response.ContentType = response.Content.Headers.ContentType.ToString();
            return response.AsActionResult();
        }

        private async Task<ActionResult> AutoRespondIfPossibleAsync()
        {
            if (await ProviderEndpoint.PendingRequest.IsReturnUrlDiscoverableAsync(OpenIdProvider.Channel.HostFactories, this.Response.ClientDisconnectedToken) == RelyingPartyDiscoveryResult.Success
                && User.Identity.IsAuthenticated)
            {
                if (ProviderEndpoint.PendingAuthenticationRequest != null)
                {
                    if (ProviderEndpoint.PendingAuthenticationRequest.IsDirectedIdentity)
                    {
                        ProviderEndpoint.PendingAuthenticationRequest.IsAuthenticated = true;
                        return await this.SendAssertion();
                    }
                }
               }

            return null;
        }

        private bool UserControlsIdentifier(IAuthenticationRequest authReq)
        {
            if (authReq == null)
            {
                throw new ArgumentNullException("authReq");
            }
            if (User == null || User.Identity == null)
            {
                return false;
            }
            Uri userLocalIdentifier = Util.GetUserClaimIdentifier(User.Identity.Name);
            return string.Equals(authReq.LocalIdentifier.ToString(), userLocalIdentifier.ToString(), StringComparison.OrdinalIgnoreCase) ||
                authReq.LocalIdentifier == PpidGeneration.PpidIdentifierProvider.GetIdentifier(userLocalIdentifier, authReq.Realm);
        }


    }
}