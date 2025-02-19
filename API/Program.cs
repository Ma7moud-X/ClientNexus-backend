using API.Extensions.API;
using Core.Interfaces.Repositories;
using Core.Repositories;
using Database.Models.Others;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    async (IUnitOfWork unitOfWork) =>
    {
        var officeImageUrl = await unitOfWork.OfficeImageUrls.AddAsync(new OfficeImageUrl {
                Id = Ulid.NewUlid(),
                Url = "https://example.com/image.jpg",
                ServiceProviderId = 1
        });
        await unitOfWork.SaveChangesAsync();
        
        
        return officeImageUrl;
    }
);

app.Run();
