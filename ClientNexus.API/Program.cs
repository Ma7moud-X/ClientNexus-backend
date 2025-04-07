using ClientNexus.API.Extensions;
using ClientNexus.Application.Domain;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Mapping;
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
    async (IServiceProviderService serviceProviderService) =>
    {
        return await serviceProviderService.SetAvailableForEmergencyAsync(2);
    }
);
app.MapControllers();
app.Run();
