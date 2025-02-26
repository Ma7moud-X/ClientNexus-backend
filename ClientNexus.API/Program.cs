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
    async (IFileService fileService) =>
    {
        IEnumerable<string> filesUrls;
        try
        {
            filesUrls = await fileService.GetFilesUrlsWithPrefixAsync("images/");
        }
        catch (Exception)
        {
            return [""];
        }

        return filesUrls;
    }
);

app.Run();
