using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UEMS_Update.Startup))]
namespace UEMS_Update
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
