using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BarCodeApp.Startup))]
namespace BarCodeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
