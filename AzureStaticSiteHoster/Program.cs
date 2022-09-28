using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<BlobContainerClient>((services) =>
{
    var config = services.GetService<IConfiguration>();
    var connectionStringConfig = config.GetValue<string>("AzureStorageConnectionString");
    var storageContainerConfig = config.GetValue<string>("AzureStorageContainer");
    return new BlobContainerClient(connectionStringConfig, storageContainerConfig);
});

var app = builder.Build();

app.MapGet("{*url}", async (string? url, BlobContainerClient container) =>
{
    string path = url ?? "/";
    Console.WriteLine($"GET {path}");
    if (path.EndsWith('/'))
    {
        path += "index.html";
    }
    var blob = container.GetBlobClient(path);
    if (await blob.ExistsAsync())
    {
        var stream = new MemoryStream();
        var propertiesTask = blob.GetPropertiesAsync();
        await blob.DownloadToAsync(stream);
        stream.Position = 0;
        return Results.Stream(stream, contentType: (await propertiesTask).Value.ContentType);
    }
    return Results.NotFound("Not found");
});

app.Run();
