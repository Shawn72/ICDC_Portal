using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ICDC_Portal.Startup))]
namespace ICDC_Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
