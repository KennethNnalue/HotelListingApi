
using System.Text;
using HotelListingAPI.Configurations;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Repositories;
using HotelListingAPI.Entities;
using HotelListingAPI.Interfaces;
using HotelListingAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace HotelListingAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<HotelListingDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        //Add IdentityCore of type User entity inheriting IdentityUser
        // Add Roles to allow us assign roles to user
        // Add Token Provider 
        // Also add the DbContext for the user
        builder.Services.AddIdentityCore<User>()
        .AddRoles<IdentityRole>()
        .AddTokenProvider<DataProtectorTokenProvider<User>>("HotelListingApi")
        .AddEntityFrameworkStores<HotelListingDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add our CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
        });

        //Add Versioning
        builder.Services.AddApiVersioning( options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver")
            );
        });

        builder.Services.AddVersionedApiExplorer( options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });


        //Add Serilog , ctx = context, lc = logger configuaration . then go to appsettings.json to set the config for serilog
        builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration)); 
        
        builder.Services.AddAutoMapper(typeof(MapperConfig));
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
        builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();
        builder.Services.AddScoped<IUserAuthManagerRepository, UserAuthManagerRepository>();
        builder.Services.AddAuthentication(options =>{
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>{
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime =true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                     builder.Configuration["JwtSettings:Key"]
                ))
            };
        });

        builder.Services.AddResponseCaching(options =>
        { options.MaximumBodySize = 1024;
          options.UseCaseSensitivePaths = true;
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseHttpsRedirection();

        //Tell app to yse our cors policy
        app.UseCors("AllowAll");
        app.UseResponseCaching();
        app.Use(async (context, next) =>
        {
            context.Response.GetTypedHeaders().CacheControl =
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(15) //Time span to fetch another data from server
            };
            context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = 
            new string[] {"Accept-Encoding"};
            await next();
        });
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

