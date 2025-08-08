using System.Text;
using Airbnb.Hubs;
using Airbnb.Middleware;
using Airbnb.Services;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using Application.Services.Chat;
using Infrastructure.Common.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

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
                        .SetIsOriginAllowed(_=> true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });

                options.AddPolicy("AllowTrusted", policy =>
                {
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                              //.WithHeaders("Authorization", "Content-Type", "X-Requested-With");
                    }
                });

                options.AddPolicy("Prod", policy =>
                {
                    policy.WithOrigins("https://ahmed-aladl.github.io")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                              //.WithHeaders("Authorization", "Content-Type", "X-Requested-With");
                });
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
        }

        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)

        {

            AddCors(services, configuration);
            ConfigureJwt(services, configuration);


            services.AddSingleton<IUserConnectionService, UserConnectionService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, GmailEmailService>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<PaymentService>();
            services.AddScoped<PropertyViolationService>();



            services.AddScoped<WishlistService>();
            services.AddScoped<ReviewService>();
            services.AddScoped<PropertyService>();
            services.AddScoped<BookingService>();
            services.AddScoped<CalendarService>();
            services.AddScoped<AmenityService>();
            services.AddScoped<PropertyTypeService>();
            services.AddScoped<HostReplyService>();



            services.AddAutoMapper(c => c.AddProfile<PropertyProfile>(), typeof(CalendarMappingProfile).Assembly);

            services.AddSwaggerGen(c => c.OperationFilter<FileUploadOperationFilter>());

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            // Add AutoMapper
            services.AddAutoMapper(
                cfg => cfg.AddProfile<CalendarMappingProfile>(),
                typeof(CalendarMappingProfile).Assembly
            );


            return services;
        }

        public static WebApplication AddPresentationDevelopmentDI(this WebApplication app)
        {

            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(op =>
            {
                op.SwaggerEndpoint("/swagger/v1/swagger.json", "Airbnb API v1");
                op.RoutePrefix = "swagger";
            });

            //app.UseCors("AllowAngularApp");

            return app;
        }

        // private methods 
        private static IServiceCollection ConfigureJwt(IServiceCollection services, IConfiguration config)
        {
            var validationParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
            };
            services.AddSingleton(validationParameter);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = validationParameter;
            });

            services.AddHttpContextAccessor();
            return services;
        }
    }
}
