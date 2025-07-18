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
using Infrastructure.Data;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;

namespace Airbnb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.ConfigureRateLimiting(builder.Configuration);
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressMapClientErrors = true;
            });

            builder.Services.AddControllers(options =>
            {
                //options.Filters.Add<ErrorHandlingFilter>();
            });

            //builder.Services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.SuppressModelStateInvalidFilter = true;
            //});

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDomain();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddPresentation(builder.Configuration);
            //builder.Services.AddIdentity<User, IdentityRole>()
            //.AddEntityFrameworkStores<AirbnbContext>()
            //     .AddDefaultTokenProviders();



            var app = builder.Build();
            app.UseIpRateLimiting();
            await DbSeeder.SeedAsync(app);


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