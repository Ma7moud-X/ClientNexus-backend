using ClientNexus.API.Extensions;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileService();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
    () =>
    {
        return "Hello World!";
    }
);

app.Run();
