using Airbnb.Middleware;
using Airbnb.Services;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using Infrastructure.Common.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Airbnb.DependencyInjection.PresentationDI
{
    public static class InfrastructureServicesRegisteration
    {
        private static IServiceCollection AddCors(
            IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );

                options.AddPolicy(
                    "AllowTrusted",
                    policy =>
                    {
                        var allowedOrigins = configuration
                            .GetSection("Cors:AllowedOrigins")
                            .Get<string[]>();

                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials() // For cookies
                            .WithHeaders("Authorization", "Content-Type", "X-Requested-With");
                    }
                );
            });
        }

        public static IServiceCollection AddPresentation(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            AddCors(services, configuration);

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, GmailEmailService>();


            services.AddScoped<WishlistService>();

            services.AddScoped<IFileService, FileService>();
            services.AddSwaggerGen(c => c.OperationFilter<FileUploadOperationFilter>());

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // Add AutoMapper
            services.AddAutoMapper(
                cfg => cfg.AddProfile<CalendarMappingProfile>(),
                typeof(CalendarMappingProfile).Assembly
            );
            services.AddAutoMapper(
                cfg => cfg.AddProfile<AmenityMappingProfile>(),
                typeof(AmenityMappingProfile).Assembly
            );

            services.AddScoped<PropertyService>();
            services.AddScoped<BookingService>();
            services.AddScoped<CalendarService>();
            services.AddScoped<AmenityService>();
            return services;
        }

        public static WebApplication AddPresentationDevelopmentDI(this WebApplication app)
        {
            app.MapHub<NotificationHub>("/notificationHub");

            app.MapOpenApi();
            app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
            return app;
        }
    }
}
