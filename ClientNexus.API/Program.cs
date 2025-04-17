using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ClientNexus.API.Extensions;
using ClientNexus.API.Utilities.SwaggerUtilities;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Mapping;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Entities.Users;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure;
using ClientNexus.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IClientService = ClientNexus.Application.Interfaces.IClientService;

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
builder.Services.AddOfferListenerServices();
builder.Services.AddScoped<IOfferSaverService, OfferSaverService>();
builder.Services.AddScoped<IBaseUserService, BaseUserService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<IEmergencyCaseService, EmergencyCaseService>();
builder.Services.AddScoped<IBaseServiceService, BaseServiceService>();
builder.Services.AddScoped<ISlotService, SlotService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddTransient(provider => new Lazy<IAppointmentService>(
    () => provider.GetRequiredService<IAppointmentService>()
));
builder.Services.AddScoped<IQuestionService, QuestionService>();

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

// NEW - Configure Identity with BaseUser
builder
    .Services.AddIdentity<BaseUser, IdentityRole<int>>()
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
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),
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
            },
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "IsClient",
        policy =>
        {
            policy.RequireClaim(ClaimTypes.Role, UserType.Client.ToString());
        }
    );

    options.AddPolicy(
        "IsServiceProvider",
        policy =>
        {
            policy.RequireClaim(ClaimTypes.Role, UserType.ServiceProvider.ToString());
        }
    );

    options.AddPolicy(
        "IsAdmin",
        policy =>
        {
            policy.RequireClaim(ClaimTypes.Role, UserType.Admin.ToString());
        }
    );
    options.AddPolicy(
       "IsClientOrAdmin",
       policy =>
       {
           policy.RequireAssertion(context =>
               context.User.HasClaim(ClaimTypes.Role, UserType.Client.ToString()) ||
               context.User.HasClaim(ClaimTypes.Role, UserType.Admin.ToString()));
       });
    options.AddPolicy(
       "IsServiceProviderOrAdmin",
       policy =>
       {
           policy.RequireAssertion(context =>
               context.User.HasClaim(ClaimTypes.Role, UserType.ServiceProvider.ToString()) ||
               context.User.HasClaim(ClaimTypes.Role, UserType.Admin.ToString()));
       });
    options.AddPolicy(
       "IsServiceProviderOrClient",
       policy =>
       {
           policy.RequireAssertion(context =>
               context.User.HasClaim(ClaimTypes.Role, UserType.ServiceProvider.ToString()) ||
               context.User.HasClaim(ClaimTypes.Role, UserType.Client.ToString()));
       });

});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        }
    );

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );

    option.OperationFilter<AuthorizeOperationFilter>();
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
            _ => StatusCodes.Status500InternalServerError,
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { error = exception?.Message })
        );
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");

// NEW - Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
