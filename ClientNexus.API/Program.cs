using ClientNexus.API.Extensions;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Mapping;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Entities.Users;
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
    async (IUnitOfWork unitOfWork) =>
    {
        var admin = await unitOfWork.Admins.GetByIdAsync(7);

        
        return admin;
    }
);
app.MapControllers();
app.Run();
