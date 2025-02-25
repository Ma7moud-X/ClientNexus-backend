using ClientNexus.API.Extensions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Infrastructure.Repositories;
using ClientNexus.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddS3Storage();
builder.Services.AddFileUploadService();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.MapGet(
    "/",
    async (IFileUploadService fileUploadService) =>
    {
        using (FileStream fileStream = new FileStream("../LICENSE", FileMode.Open))
        {
            await fileUploadService.UploadFileAsync(fileStream, "gg", "plain/text");
        }
        
        
    }
);

app.Run();
