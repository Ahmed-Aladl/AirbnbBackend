

using Microsoft.EntityFrameworkCore;

namespace Airbnb.DependencyInjection.DomainDI
{
    public static class DomainServicesRegistraion
    {
        public static IServiceCollection AddDomain(this IServiceCollection services,IConfiguration configuration)
        {
            //services.AddDbContext<GymContext>(op => op.UseSqlServer(configuration.GetConnectionString(configuration.GetSection("AirbnbConnectionString").Value)));

            return services;

        }
    }
}
