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
    async (
        IEmergencyCaseService emergencyCaseService,
        IServiceProviderService serviceProviderService,
        IOfferService offerService,
        IMapService mapService,
        IGeneralOfferListenerService generalOfferListenerService
    ) =>
    {
        // if (await emergencyCaseService.HasActiveEmergencyAsync(2))
        // {
        //     return Results.BadRequest("Got an active emergency case.");
        // }

        // --> Service Provider adding his location
        await emergencyCaseService.SetServiceProviderLocationAsync(
            2,
            30.05054534201179,
            31.24686175014378
        );

        // --> Client initiating an emergency case
        var emergencyCase = await emergencyCaseService.InitiateEmergencyCaseAsync(
            new CreateEmergencyCaseDTO
            {
                Name = "Test Emergency",
                Description = "Test Description",
                MeetingLongitude = 30.051097049710517,
                MeetingLatitude = 31.247516839119765,
            },
            2,
            "Mahmoud",
            "Abuelnaga"
        );

        await Task.Delay(1000 * 30);
        // --> Service Provider sending an offer

        var providerLocation = await emergencyCaseService.GetServiceProviderLocationAsync(2);
        if (providerLocation == null)
        {
            return Results.BadRequest("Service provider location not found.");
        }

        var meetingLocation = await emergencyCaseService.GetMeetingLocationAsync(emergencyCase.Id);
        if (meetingLocation == null)
        {
            return Results.BadRequest("Meeting location not found.");
        }

        TravelDistance travelDistance = await mapService.GetTravelDistanceAsync(
            providerLocation.Value,
            meetingLocation.Value,
            ClientNexus.Application.Enums.TravelProfile.Walk
        );

        await offerService.CreateOfferAsync(
            emergencyCase.Id,
            100m,
            (await serviceProviderService.GetServiceProviderOverviewAsync(2))!,
            travelDistance,
            TimeSpan.FromMinutes(1)
        );

        // --> Client getting offer
        await generalOfferListenerService.SubscribeAsync(emergencyCase.Id);
        var offer = await generalOfferListenerService.ListenAsync(CancellationToken.None);

        // --> Client accepting the offer
        await Task.Delay(1000 * 10);
        await offerService.AcceptOfferAsync(emergencyCase.Id, 2, 2);

        return Results.Ok("Emergency case initiated and offer created successfully.");
    }
);
app.MapControllers();
app.Run();
