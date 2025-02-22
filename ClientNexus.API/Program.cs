using ClientNexus.API.Extensions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    (IUnitOfWork unitOfWork) =>
    {
        return "Hello World";
    }
);

app.Run();
