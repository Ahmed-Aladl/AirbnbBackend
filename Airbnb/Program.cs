using Airbnb.DependencyInjection.ApplicationDI;
using Airbnb.DependencyInjection.DomainDI;
using Airbnb.DependencyInjection.InfrastructureDI;
using Airbnb.Middleware;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;

using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using AutoMapper;
using Infrastructure.Common.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Airbnb.DependencyInjection.PresentationDI;

namespace Airbnb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.ConfigureRateLimiting(builder.Configuration);


            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ErrorHandlingFilter>();
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDomain();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddPresentation(builder.Configuration);

            

            var app = builder.Build();
            app.UseIpRateLimiting();

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