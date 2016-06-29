using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(example.Startup))]
namespace example
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
