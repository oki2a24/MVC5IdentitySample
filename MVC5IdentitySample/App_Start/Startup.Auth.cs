using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MVC5IdentitySample.Models;
using Owin;

namespace MVC5IdentitySample
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            // 1 要求につき 1 インスタンスのみを使用するように DB コンテキスト、ユーザー マネージャー、サインイン マネージャーを構成します。
            app.CreatePerOwinContext(MVC5IdentitySampleContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Users/Login")
            });
        }
    }
}