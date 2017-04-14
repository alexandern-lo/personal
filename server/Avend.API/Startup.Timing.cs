using Microsoft.AspNetCore.Builder;

namespace Avend.API
{
    public partial class Startup
    {
        protected virtual void ConfigureTiming(IApplicationBuilder appBuilder)
        {
            //add request timing middleware here
        }
    }
}
