using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DragonsClaw.Startup))]
namespace DragonsClaw
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
