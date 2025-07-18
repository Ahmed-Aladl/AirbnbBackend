
using Airbnb.Services;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Airbnb.DependencyInjection.PresentationDI
{
    public static class InfrastructureServicesRegisteration
    {
        private static IServiceCollection AddCors(IServiceCollection services,IConfiguration configuration)
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

                            policy.WithOrigins(allowedOrigins)
                                  .AllowAnyMethod()
                                  .AllowAnyHeader()
                                  .AllowCredentials() // For cookies
                                  .WithHeaders("Authorization", "Content-Type", "X-Requested-With");
                        });

                    });

        }
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {

            AddCors(services, configuration);
            services.AddScoped<IFileService,FileService>();
            services.AddSwaggerGen(c =>
                          c.OperationFilter<FileUploadOperationFilter>()
            );



            services.AddScoped<PropertyService>();
            services.AddScoped<CalendarService>();
            return services;
        }


        public static WebApplication AddPresentationDevelopmentDI(this WebApplication app) 
        {
            app.MapOpenApi();
            app.UseSwaggerUI(op => op.SwaggerEndpoint("/openapi/v1.json", "v1"));
            return app;
        }
    }
}
