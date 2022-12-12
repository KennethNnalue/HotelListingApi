﻿
using System.Text;
using HotelListingAPI.Configurations;
using HotelListingAPI.Data;
using HotelListingAPI.Data.Repositories;
using HotelListingAPI.Entities;
using HotelListingAPI.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //Tell app to yse our cors policy
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

