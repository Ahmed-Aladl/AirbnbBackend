using Airbnb.Hubs;
using Application.Interfaces;
using Application.Interfaces.Hubs;
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
            // ChatNotifier for chat hub
            services.AddScoped<IChatNotifier, ChatNotifier>();
            services.AddScoped<ICalendarAvailabilityRepo, CalendarAvailabilityRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>(); // <-- Register UnitOfWork in DI
            services.AddScoped<IAmenityRepo, AmenityRepo>();

            // Add AutoMapper
            services.AddAutoMapper(
                config =>
                {
                    config.AddProfile<PropertyProfile>();
                    config.AddProfile<WishlistProfile>();
                    config.AddProfile<PaymentProfile>();
                },
                typeof(PropertyProfile).Assembly
            );

            services
                .AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AirbnbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
