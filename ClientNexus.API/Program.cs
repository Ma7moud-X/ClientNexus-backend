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
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure;
using ClientNexus.Infrastructure.Repositories;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IClientService = ClientNexus.Application.Interfaces.IClientService;

//DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileService();
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    return DbTryCatchDecorator<IUnitOfWork>.Create(new UnitOfWork(context));
});
builder.Services.AddSingleton<IPushNotification, ExpoPushNotification>();
builder.Services.AddRedisCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
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
builder.Services.AddScoped<IAvailableDayService, AvailableDayService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddTransient(provider => new Lazy<IAppointmentService>(() =>
    provider.GetRequiredService<IAppointmentService>()
));
builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPhoneNumberService, PhoneNumberService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IServiceProviderService, ServiceProviderService>();
builder.Services.AddScoped<ClientService>(); // FIX: Register ClientService directly
builder.Services.AddScoped<IClientService, ClientService>(); // Optionally, you can keep the interface binding
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAdmainService, AdmainService>();
builder.Services.AddScoped<ISpecializationService, SpecializationService>();

builder.Services.AddScoped<IcountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityServicecs, CityService>();
builder.Services.AddScoped<IServiceProviderTypeService, serviceProviderTypeService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();

builder.Services.AddTransient<IOtpService, OtpService>();
builder.Services.AddScoped<ServiceProviderService>(); // FIX: Register ServiceProviderService
builder.Services.AddScoped<PaymentService>();

builder.Services.AddScoped<PaymobPaymentService>(sp => new PaymobPaymentService(
    secretKey: builder.Configuration["Paymob:SecretKey"],
    publicKey: builder.Configuration["Paymob:PublicKey"],
    paymentMethodIds: builder.Configuration.GetSection("Paymob:PaymentMethodIds").Get<int[]>(),
    redirectionUrl: builder.Configuration["Paymob:RedirectionUrl"],
    notificationUrl : builder.Configuration["Paymob:NotificationUrl"]

));

builder.Services.AddTransient<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IProblemService, ProblemService>();

builder.Services.AddScoped<IcountryService, CountryService>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<ICityServicecs, CityService>();
builder.Services.AddScoped<IServiceProviderTypeService, serviceProviderTypeService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

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
                context.User.HasClaim(ClaimTypes.Role, UserType.Client.ToString())
                || context.User.HasClaim(ClaimTypes.Role, UserType.Admin.ToString())
            );
        }
    );
    options.AddPolicy(
        "IsServiceProviderOrAdmin",
        policy =>
        {
            policy.RequireAssertion(context =>
                context.User.HasClaim(ClaimTypes.Role, UserType.ServiceProvider.ToString())
                || context.User.HasClaim(ClaimTypes.Role, UserType.Admin.ToString())
            );
        }
    );
    options.AddPolicy(
        "IsServiceProviderOrClient",
        policy =>
        {
            policy.RequireAssertion(context =>
                context.User.HasClaim(ClaimTypes.Role, UserType.ServiceProvider.ToString())
                || context.User.HasClaim(ClaimTypes.Role, UserType.Client.ToString())
            );
        }
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v2", new OpenApiInfo { Title = "Demo API", Version = "v2" });
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
    // NEW - Swagger Configuration
    //if (builder.Environment.IsDevelopment())
    //{
    //    builder.Services.AddEndpointsApiExplorer();
    //    builder.Services.AddSwaggerGen();

    //}

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
            ServerException => StatusCodes.Status500InternalServerError,
            NotFoundException => StatusCodes.Status404NotFound,
            NotAllowedException => StatusCodes.Status400BadRequest,
            ExpiredException => StatusCodes.Status400BadRequest,
            InvalidInputException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

        if (exception is ServerException)
        {
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new { error = "Internal Server Error" })
            );
            return;
        }

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { error = exception?.Message })
        );
    });
});

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "ASP.NET Web API v2");
});

//}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllOrigins");

// NEW - Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add Hangfire dashboard and configure jobs
app.UseHangfireConfiguration();
app.UseHangfireDashboard("/hangfire"); // no authorization


app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
