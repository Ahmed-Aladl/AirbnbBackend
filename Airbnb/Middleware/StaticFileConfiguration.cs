using Microsoft.AspNetCore.StaticFiles;

namespace Airbnb.Middleware
{
    public static class StaticFileConfiguration
    {
        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
        {
            var provider = new FileExtensionContentTypeProvider();

            // Add or override image MIME types
            provider.Mappings[".avif"] = "image/avif";
            provider.Mappings[".jpeg"] = "image/jpeg";
            provider.Mappings[".jpg"] = "image/jpeg";
            provider.Mappings[".png"] = "image/png";

            return app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
        }
    }
}
