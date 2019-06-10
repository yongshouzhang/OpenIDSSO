using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using CSRedis;
using DotNetOpenAuth.OpenId.Provider;

namespace OpenIDSSO.Controllers
{
    using Services;
    using Models;
    using Infrastructure;
    using Common;
    public class AccountController : Controller
    {
        private readonly IUserModelService _userService;
        private readonly ILoginLogService _logService;
        private readonly IFormsAuthentication _formsAuthService;

        private  IHostProcessedRequest RpRequest
        {
            get { return  ProviderEndpoint.PendingRequest; }
        }

        public AccountController(IUserModelService userService,
            IFormsAuthentication formsAuthService, 
            ILoginLogService logService)
        {
            this._userService = userService;
            this._logService = logService;
            this._formsAuthService = formsAuthService;
        }
        public ActionResult LogOn()
        {
            return View();
        }

        public ActionResult LogOff(string returnUrl=null)
        {
            var masterSession = RedisHelper.GetLoginSession(User.Identity.Name + ":session", Util.GetRootedUri("").ToString());
            if (masterSession == Session.SessionID)
            {
                _formsAuthService.SignOut(User.Identity.Name, false);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            return Redirect("/Home/Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            UserModel user = null;
            if (!ValidateLogOn(userName, password,out user))
            {
                return View();
            }

            string loginSession = RedisHelper.GetLoginSession(user.UserName + ":session", Util.GetRootedUri("").ToString());

            if(!string.IsNullOrEmpty(loginSession)
                && !string.Equals(loginSession, Session.SessionID, StringComparison.Ordinal))
            {
                Session["_askuser_"] = true;
                Session["_tempuser_"] = user.UserName;
                Session["_remember_"] = rememberMe;
                //转跳 到询问页面
                return this.RedirectToAction("askuser", "openid");
            }

            _formsAuthService.SignIn(user, rememberMe);
            string claimIdentifier = Util.GetUserClaimIdentifier(userName).ToString();
            _logService.Add(new LoginLogModel
            {
                ID = Guid.NewGuid(),
                AddTime = DateTime.Now,
                Flag = 0,
                UserName = user.UserName,
                OpenIdClaimedIdentifier = claimIdentifier,
                OpenIdFriendlyIdentifier = claimIdentifier
            });

            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);

         }

        private bool ValidateLogOn(string userName,string password,out UserModel loginUser)
        {
            loginUser = null;
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("errorMsg", "用户名为空");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("errorMsg", "密码为空");
                return false;
            }
            UserModel user = _userService.Get(t => t.UserName == userName);

            if (user == null)
            {
                ModelState.AddModelError("errorMsg", "用户名不存在");
                return false;
            }
            string pwdhash = MD5Helper.ComputeMD5((userName + password).Trim());
            if (user.Password != pwdhash)
            {
                ModelState.AddModelError("errorMsg", "用户名与密码不匹配");
                return false;
            }
            loginUser = user;
            return true;
        }

        public ActionResult Register()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post),ValidateAntiForgeryToken]
        public ActionResult Register(string username, string useremail, string passwd, string confirmpasswd)
        {
            if (!ValidateRegistration(username, useremail, passwd, confirmpasswd))
            {
                return View();
            }
            var user = new UserModel
            {
                UserName = username,
                Email = useremail,
                Password = MD5Helper.ComputeMD5((username + passwd).Trim()),
                Birthday = DateTime.Now,
                RegDate = DateTime.Now,
                Gender = 'F',
            };

            if (_userService.Exists(user))
            {
                ModelState.AddModelError("errorMsg", " 该用户已存在");
                return View();
            }
            _userService.Add(user);
            RedisHelper.SetUserInfo(user.UserName + ":master", user);
            string claimIdentifier = Util.GetUserClaimIdentifier(username).ToString();
            _logService.Add(new LoginLogModel
            {
                UserName = user.UserName,
                AddTime = DateTime.Now,
                Flag = 1,
                ID = Guid.NewGuid(),
                OpenIdClaimedIdentifier = claimIdentifier,
                OpenIdFriendlyIdentifier = claimIdentifier
            });
            _formsAuthService.SignIn(user, false);
            return RedirectToAction("Index", "Home");
        }

        private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("errorMsg", "用户名不可为空");
                return false;
            }
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("errorMsg", "电子邮箱不可为空");
                return false;
            }
            if (password == null || password.Length < 8)
            {
                ModelState.AddModelError("errorMsg", "密码长度至少8位");
                return false;
            }
            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("errorMsg", "两次密码不一致");
                return false;
            }
            return true;
        }

    }
}