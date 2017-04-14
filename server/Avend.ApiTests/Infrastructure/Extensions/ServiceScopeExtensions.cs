using Microsoft.Extensions.DependencyInjection;

namespace Avend.ApiTests.Infrastructure.Extensions
{
    public static class ServiceScopeExtensions
    {
        public static T GetService<T>(this IServiceScope scope)
        {
            return (T)scope.ServiceProvider.GetService(typeof(T));
        }
    }
}