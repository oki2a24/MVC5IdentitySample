using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC5IdentitySample.Startup))]
namespace MVC5IdentitySample
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}