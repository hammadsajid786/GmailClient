using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebMailClient.Startup))]
namespace WebMailClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
