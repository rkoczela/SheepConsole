using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSheepshead.Startup))]
namespace WebSheepshead
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
