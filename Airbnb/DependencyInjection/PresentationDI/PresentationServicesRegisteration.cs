
namespace Airbnb.DependencyInjection.PresentationDI
{
    public static class InfrastructureServicesRegisteration
    {

        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
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
