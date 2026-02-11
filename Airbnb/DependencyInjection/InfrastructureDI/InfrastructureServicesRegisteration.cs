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
            {
                op.UseSqlServer(
                    configuration.GetConnectionString(
                        configuration?.GetSection("AirbnbConnectionString")?.Value ?? "default"
                    )
                );
                // Suppress the pending migrations error in EF Core 9
                op.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });
            return services;
        }
    }
}
