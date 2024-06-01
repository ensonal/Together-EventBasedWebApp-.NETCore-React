using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Together.Contracts;
using Together.Core.DTO;
using Together.DataAccess;
using Together.Service;

namespace Together.DependencyInjection;

public static class ServiceRegistration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TogetherDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Server=tcp:together-app.database.windows.net,1433;Initial Catalog=TogetherDb;Persist Security Info=False;User ID=togetheradmin;Password={151Sq+++};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;")));
        services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<TogetherDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<ISportService, SportService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IAzureStorageService, AzureStorageService>();
        services.AddScoped<IFilterService, FilterService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IRequestManagementService, RequestManagementService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        services.AddSignalR();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,                       
                        ValidIssuer = configuration["JWTSettings:Issuer"],
                        ValidAudience = configuration["JWTSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                    };
                   o.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = c =>
                        {
                            c.NoResult();
                            c.Response.StatusCode = 500;
                            c.Response.ContentType = "text/plain";
                            return c.Response.WriteAsync(c.Exception.ToString());
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(("You are not authorized to access this resource"));
                            return context.Response.WriteAsync(result);
                        }
                    }; 
                });
    }
}