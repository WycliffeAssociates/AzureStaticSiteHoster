using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<BlobContainerClient>((services) =>
{
    var config = services.GetRequiredService<IConfiguration>();
    var connectionStringConfig = config.GetValue<string>("AzureStorageConnectionString")
        ?? throw new InvalidOperationException("AzureStorageConnectionString is not configured");
    var storageContainerConfig = config.GetValue<string>("AzureStorageContainer")
        ?? throw new InvalidOperationException("AzureStorageContainer is not configured");
    return new BlobContainerClient(connectionStringConfig, storageContainerConfig);
});

var app = builder.Build();

app.MapGet("{*url}", async (string? url, BlobContainerClient container) =>
{
    var path = url ?? "/";
    Console.WriteLine($"GET {path}");
    if (path.EndsWith('/'))
    {
        path += "index.html";
    }
    var blob = container.GetBlobClient(path);
    if (await blob.ExistsAsync())
    {
        var download = await blob.DownloadStreamingAsync();
        var contentType = download.Value.Details.ContentType ?? "application/octet-stream";
        return Results.Stream(download.Value.Content, contentType: contentType);
    }
    return Results.NotFound("Not found");
});

app.Run();
