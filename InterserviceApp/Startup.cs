using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InterserviceApp.Startup))]
namespace InterserviceApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
