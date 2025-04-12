using ClientNexus.API.Extensions;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Mapping;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure;
using ClientNexus.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;
using ClientNexus.Domain.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using IClientService = ClientNexus.Application.Interfaces.IClientService;
using Microsoft.OpenApi.Models;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileService();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPushNotification, FirebasePushNotification>();
builder.Services.AddRedisCache();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpService, HttpService>();
builder.Services.AddMapService();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<ISlotService, SlotService>(); // Register the service
builder.Services.AddOfferListenerServices();
builder.Services.AddScoped<IOfferSaverService, OfferSaverService>();
builder.Services.AddScoped<IBaseUserService, BaseUserService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IEmergencyCaseService, EmergencyCaseService>();
builder.Services.AddScoped<IBaseServiceService, BaseServiceService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddTransient(provider => new Lazy<IAppointmentService>(() => provider.GetRequiredService<IAppointmentService>()));
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPhoneNumberService, PhoneNumberService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdmainService, AdmainService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();
builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddTransient<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();


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

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation
    swagger.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
    });
    
    // To Enable authorization using Swagger (JWT)
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });
    
    // Add security requirement to globally apply Bearer token authentication
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        // Log exception for debugging
        Console.WriteLine($"Exception caught: {exception?.Message}");

        context.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = exception?.Message }));

    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "ASP.NET Web API v2");
    });
}

app.UseHttpsRedirection();
app.UseRouting();

// NEW - Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();