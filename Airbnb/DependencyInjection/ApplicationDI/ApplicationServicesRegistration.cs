
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Airbnb.DependencyInjection.ApplicationDI
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICalendarAvailabilityRepo, CalendarAvailabilityRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(); // <-- Register UnitOfWork in DI


            // Add AutoMapper
            services.AddAutoMapper(config =>
            {
                config.AddProfile<PropertyProfile>();
                config.AddProfile<WishlistProfile>();
            }, typeof(PropertyProfile).Assembly);

            services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AirbnbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
