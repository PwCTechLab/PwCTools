using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PwCTools.Startup))]
namespace PwCTools
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app.MapSignalR());
        }
    }
}
