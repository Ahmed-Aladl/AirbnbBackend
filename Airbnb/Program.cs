using System.Security.Claims;
using Airbnb.DependencyInjection.ApplicationDI;
using Airbnb.DependencyInjection.DomainDI;
using Airbnb.DependencyInjection.InfrastructureDI;
using Airbnb.DependencyInjection.PresentationDI;
using Airbnb.Hubs;
using Airbnb.Middleware;
using Application.DTOs.GroqRequestDto;
using Application.Interfaces.IRepositories;
using Application.Mappings;
using Application.Services;
using AspNetCoreRateLimit;
using AutoMapper;
using Domain.Models;
using DotNetEnv;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Stripe;


namespace Airbnb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            DotNetEnv.Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables();

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

            builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection("Groq"));
            builder.Services.AddHttpClient();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDomain();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddPresentation(builder.Configuration);

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("Prod");
            app.UseCors("AllowAngularApp");
            app.UseCors("AllowTrusted");

            //await DbSeeder.SeedAsync(app);
            app.UseIpRateLimiting();

            app.UseRouting();

            if (app.Environment.IsDevelopment())
            {
                app.AddPresentationDevelopmentDI();
            }

            app.UseMiddleware<JwtFromCookieMiddleware>();
            app.UseMiddleware<TokenValidationMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapHub<ChatHub>("/chatHub");
            app.MapHub<NotificationHub>("/notificationHub");
            app.MapControllers();
            app.Use(
                async (context, next) =>
                {
                    Console.WriteLine($"\n\nRequest Path: {context.Request.Path}");
                    var user = context.User;
                    if (user?.Identity?.IsAuthenticated == true)
                    {
                        Console.WriteLine($"User: {user.Identity.Name}");

                        foreach (var claim in user.Claims)
                        {
                            Console.WriteLine($"\nClaim: {claim.Type} = {claim.Value}");
                        }
                    }
                    else
                        Console.WriteLine(
                            "******\n\n8\n8\n8\n8\n8\n8\n no data found\n8\n8\n8\n8\n8\n8\n8\n\n\n"
                        );
                    Console.WriteLine("==== Request Headers ====");
                    foreach (var header in context.Request.Headers)
                    {
                        Console.WriteLine($"\n{header.Key}: {header.Value}");
                    }
                    Console.WriteLine("\n\n");

                    Console.WriteLine("==== Request Cookies ====");
                    foreach (var cookie in context.Request.Cookies)
                    {
                        Console.WriteLine($"\n{cookie.Key}: {cookie.Value}");
                    }
                    Console.WriteLine("\n\n");

                    await next(context);
                }
            );

            app.Run();
        }
    }
}
