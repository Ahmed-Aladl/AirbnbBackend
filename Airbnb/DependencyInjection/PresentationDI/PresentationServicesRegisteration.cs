using Airbnb.Services;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using Infrastructure.Common.Repositories;
using Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Airbnb.DependencyInjection.PresentationDI
{
    public static class InfrastructureServicesRegisteration
    {
        private static IServiceCollection AddCors(IServiceCollection services, IConfiguration configuration)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });

                options.AddPolicy("AllowTrusted", policy =>
                {
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials()
                              .WithHeaders("Authorization", "Content-Type", "X-Requested-With");
                    }
                });

                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }

        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            AddCors(services, configuration);

            services.AddScoped<WishlistService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<PropertyService>();
            services.AddScoped<BookingService>();
            services.AddScoped<CalendarService>();
            services.AddScoped<AmenityService>();

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<FileUploadOperationFilter>();
            });

            services.AddAutoMapper(typeof(CalendarMappingProfile).Assembly);
            services.AddAutoMapper(typeof(AmenityMappingProfile).Assembly);

            return services;
        }

        public static WebApplication AddPresentationDevelopmentDI(this WebApplication app)
        {
            app.MapOpenApi();
            app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));

            app.UseCors("AllowAngularApp"); 

            return app;
        }
    }
}
