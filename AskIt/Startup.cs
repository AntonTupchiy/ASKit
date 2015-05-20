using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AskIt.Startup))]
namespace AskIt
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
