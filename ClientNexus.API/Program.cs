using ClientNexus.API.Extensions;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileUploadService();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    () =>
    {
        return "Hello World!";
    }
);

app.Run();
