using Airbnb.DependencyInjection.ApplicationDI;
using Airbnb.DependencyInjection.DomainDI;
using Airbnb.DependencyInjection.InfrastructureDI;
using Airbnb.DependencyInjection.PresentationDI;

using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using AutoMapper;
using Infrastructure.Common.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Airbnb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDomain();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddPresentation();

            

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.AddPresentationDevelopmentDI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}