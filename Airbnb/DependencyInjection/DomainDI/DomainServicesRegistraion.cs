

using Microsoft.EntityFrameworkCore;

namespace Airbnb.DependencyInjection.DomainDI
{
    public static class DomainServicesRegistraion
    {
        public static IServiceCollection AddDomain(this IServiceCollection services,IConfiguration configuration)
        {

            return services;

        }
    }
}
