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
    async (IUnitOfWork unitOfWork) =>
    {
        AccessLevel? accessLevel = await unitOfWork.AccessLevels.GetByIdAsync(1);
        unitOfWork.AccessLevels.Delete(accessLevel!);
        await unitOfWork.SaveChangesAsync();
        return "success";
    }
);

app.Run();
