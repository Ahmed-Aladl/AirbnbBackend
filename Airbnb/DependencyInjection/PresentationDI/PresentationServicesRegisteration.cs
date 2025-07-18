
using Application.Mappings;
using Application.Services;

namespace Airbnb.DependencyInjection.PresentationDI
{
    public static class InfrastructureServicesRegisteration
    {

        public static IServiceCollection AddPresentation(this IServiceCollection services)

        {
            // Add AutoMapper
            services.AddAutoMapper(typeof(CalendarMappingProfile).Assembly);

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
