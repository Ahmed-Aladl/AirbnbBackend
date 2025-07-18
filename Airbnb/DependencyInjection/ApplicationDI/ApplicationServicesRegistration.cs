
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Services;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;

namespace Airbnb.DependencyInjection.ApplicationDI
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICalendarAvailabilityRepo, CalendarAvailabilityRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // <-- Register UnitOfWork in DI
            services.AddScoped<IAmenityRepo, AmenityRepo>();

            return services;
        }
    }
}
