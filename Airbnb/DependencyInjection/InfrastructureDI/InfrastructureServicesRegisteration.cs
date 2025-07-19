using Application.Interfaces; // <-- Add this for IUnitOfWork
using Infrastructure.Common; // <-- Add this for UnitOfWork
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Airbnb.DependencyInjection.InfrastructureDI
{
    public static class InfrastructureServicesRegisteration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<AirbnbContext>(op =>
                op.UseSqlServer(
                    configuration.GetConnectionString(
                        configuration?.GetSection("AirbnbConnectionString")?.Value ?? ""
                    )
                )
            );
            return services;
        }
    }
}
