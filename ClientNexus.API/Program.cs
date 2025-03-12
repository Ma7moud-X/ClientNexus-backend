using ClientNexus.API.Extensions;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Mapping;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Enums;
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
builder.Services.AddLocationService();
builder.Services.AddScoped<IOfferService, OfferService>();
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
    async (IEventListener eventListener, IOfferService offerService) =>
    {
        await offerService.AllowOffersAsync(
            new ServiceProviderEmergencyDTO
            {
                ServiceId = 1,
                ClientFirstName = "Test Client First Name",
                ClientLastName = "Test Client Last Name",
                Name = "Test Service Name",
                Description = "Test Service Description",
                MeetingLatitude = 10.00d,
                MeetingLongitude = 20.00d,
            }
        );

        using var listener = new OfferListener(eventListener, 1);
        _ = offerService.CreateOfferAsync(
            1,
            10.000m,
            new ClientNexus.Application.Models.ServiceProviderOverview
            {
                ServiceProviderId = 1,
                FirstName = "Test Service Provider First Name",
                LastName = "Test Service Provider Last Name",
                Rating = 4.5f,
                ImageUrl = "test_image_url",
                YearsOfExperience = 5,
            },
            new ClientNexus.Application.Domain.TravelDistance
            {
                Distance = 10,
                DistanceUnit = "meters",
                Duration = 10,
                DurationUnit = "minutes",
            }
        );

        var dto = await listener.ListenForOfferAsync(CancellationToken.None);
        Console.WriteLine($"Offer: {dto}");
    }
);
app.MapControllers();
app.Run();
