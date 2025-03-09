<<<<<<< Updated upstream
using ClientNexus.API.Extensions;

using ClientNexus.Application.Mapping;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure;
using ClientNexus.Infrastructure.Repositories;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileService();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPushNotification, FirebasePushNotification>();
builder.Services.AddRedisCache();

builder.Services.AddScoped<ISlotService, SlotService>(); // Register the service

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet(
    "/",
    () =>
    {
        return "Hello World!";
    }
);
app.MapControllers();
app.Run();
=======
﻿using ClientNexus.API.Extensions;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure.Repositories;
using ClientNexus.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileService();

// NEW: Registering UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IPushNotification, FirebasePushNotification>();

builder.Services.AddControllers();

// NEW - Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// NEW - Configure Identity with BaseUser
builder.Services.AddIdentity<BaseUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// NEW - Configure Identity options (Password complexity)
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// NEW - JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// NEW - Swagger Configuration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// NEW - Swagger setup for Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// NEW - Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.MapGet("/", () => "Hello World!"); // NEW - Restored test endpoint

app.Run();
>>>>>>> Stashed changes
