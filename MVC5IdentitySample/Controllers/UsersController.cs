using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC5IdentitySample.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MVC5IdentitySample.Controllers
{
    public class UsersController : Controller
    {
        private MVC5IdentitySampleContext db = new MVC5IdentitySampleContext();

        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public UsersController()
        {
        }

        public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        // GET: Users
        public ActionResult Index()
        {
            // TODO これだと例外。なぜ。。。
            //var users = db.Users.Select(a => new User
            //{
            //    Id = a.Id,
            //    UserName = a.UserName,
            //    Memo = a.Memo
            //}).ToList<User>();

            var users = db.Users.ToList();
            return View(users);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(User user, string returnUrl)
        {
            var userForLogin = await UserManager.FindAsync(user.UserName, user.Password);
            if (userForLogin == null)
            {
                return View(user);
            }

            await SignInManager.SignInAsync(userForLogin, false, false);

            return RedirectToLocal(returnUrl);
        }

        // POST: /Home/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Users");
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserName,Password,Memo")] User user)
        {
            if (ModelState.IsValid)
            {
                // UserStore に定義した CreateAsync(user) を呼び出してはダメ。
                // ↓だとパスワードがハッシュ化されないため NG。
                //var result = await userManager.CreateAsync(user);
                // CreateAsync(user, applicationUser.Password) を呼び出すこと！
                // インターフェースを実装したメソッドを呼び出すのがダメだったので、どのメソッドを使うべきなのかわからなくて辛い。

                var userForCreate = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = user.UserName,
                    Memo = user.Memo
                };
                var result = await UserManager.CreateAsync(userForCreate, user.Password);
                if (result.Succeeded)
                {
                    // 作成したユーザで即ログインする。
                    var signInUser = await UserManager.FindByNameAsync(userForCreate.UserName);
                    if (signInUser == null)
                    {
                        return View(user);
                    }
                    await SignInManager.SignInAsync(signInUser, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UserName,Memo")] User user)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            return View(user);
        }

        [Authorize]
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index");
            }
            AddErrors(result);
            return View(model);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                // 現在ログイン中のユーザを削除していたらログアウトする。
                var signInUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (signInUser == null)
                {
                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index");
                }
                await SignInManager.SignInAsync(signInUser, false, false);

                return RedirectToAction("Index");
            }
            AddErrors(result);
            return HttpNotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region ヘルパー
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
