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

using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;



DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);


DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", "ClientNexus.Infrastructure", ".env"));

// Read the connection string from the environment
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STR");


//if (string.IsNullOrEmpty(connectionString))
//{
//    Console.WriteLine("DB_CONNECTION_STR is not set.");
//    throw new Exception("Database connection string not found in environment variables.");
//}
//else
//{
//    Console.WriteLine($"Connection string loaded: {connectionString}");
//}

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));  // Use the connection string from the environment variable



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
builder.Services.AddScoped<ClientService>();  // FIX: Register ClientService directly
builder.Services.AddScoped<IClientService, ClientService>();  // Optionally, you can keep the interface binding
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdmainService, AdmainService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 652dfac (Add Document service)
builder.Services.AddScoped<IcountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityServicecs, CityService>();
builder.Services.AddScoped<IServiceProviderTypeService, serviceProviderTypeService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();    
<<<<<<< HEAD
=======
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddScoped<ServiceProviderService>();  // FIX: Register ServiceProviderService
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddSingleton<ICache, RedisCache>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

=======
>>>>>>> 652dfac (Add Document service)




>>>>>>> 2c84f52d7eed6f3cc896034998e97f1f99fd78af

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
    builder.Services.AddEndpointsApiExplorer();
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


app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();

