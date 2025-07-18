using Infrastructure.Contexts;
using Infrastructure.Common; // <-- Add this for UnitOfWork
using Application.Interfaces; // <-- Add this for IUnitOfWork
using Microsoft.EntityFrameworkCore;
namespace Airbnb.DependencyInjection.InfrastructureDI
{
    public static class InfrastructureServicesRegisteration
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AirbnbContext>(op => op
                                                        .UseSqlServer(
                                                                    configuration
                                                                        .GetConnectionString(
                                                                                configuration?
                                                                                    .GetSection("AirbnbConnectionString")?
                                                                                    .Value ?? "")
                                                                        )
                                                        );
            return services;
        }
    }
}