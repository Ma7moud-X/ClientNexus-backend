using API.Extensions.API;
using Core.Interfaces.Repositories;
using Core.Repositories;
using Database.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    () =>
    {
        return "Hello World";
    }
);

app.Run();
