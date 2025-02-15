using API.Extensions.API;
using Core.Interfaces.Repositories;
using Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    async (IUnitOfWork unitOfWork) =>
    {
        return "Hello World!";
    }
);

app.Run();
