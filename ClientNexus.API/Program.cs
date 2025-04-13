using ClientNexus.API.Extensions;
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
using ClientNexus.Domain.Entities.Services;
using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Services;
using IClientService = ClientNexus.Application.Interfaces.IClientService;
using Amazon.S3;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();
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
builder.Services.AddScoped<IPhoneNumberService, PhoneNumberService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IserviceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdmainService, AdmainService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();


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



        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token != null && AuthService.IsTokenRevoked(token.RawData))
                {
                    context.Fail("Token has been revoked.");
                }
            }
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

