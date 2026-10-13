# AzureStaticSiteHoster
A small web application to make static websites in local azure storage emulators much better to use

## Why does this exist
Well, I was building what is essentially a giant static site generator that uses Azure Blob Storage to store the output
and to test things locally using the Azure storage emulator and Azurite I needed a way of viewing the results through a web browser. After many months of downloading from storage and then feeding it through another http server I built this. 

And hey, working offline on an airplane using just the local emulator is pretty cool too.

## How it works
Every `GET` request is mapped straight to a blob in the configured container:

| Request | Blob served |
| --- | --- |
| `/` | `index.html` |
| `/about/` | `about/index.html` |
| `/css/site.css` | `css/site.css` |

- Paths ending in `/` get `index.html` appended.
- The blob is streamed back with the `Content-Type` stored on the blob (falling back to `application/octet-stream` if none is set), so make sure your uploads set content types correctly.
- Anything that doesn't exist returns a `404`.
- Each request is logged to the console as `GET <path>`.

## Requirements
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- An Azure Storage account, or a local emulator such as [Azurite](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)
- (Optional) Docker, if you want to run it in a container

## Building
A simple `dotnet build` or just use an IDE like Visual Studio or Rider

## Configuration
Two settings are required. They can be set in `AzureStaticSiteHoster/appsettings.json`, as environment variables, or on the command line:

| Setting | Description | Default in appsettings.json |
| --- | --- | --- |
| `AzureStorageConnectionString` | Connection string for your Azure Storage. For the local emulator just use `UseDevelopmentStorage=true` | `UseDevelopmentStorage=true` |
| `AzureStorageContainer` | The container to use as the source of the site | `web` |

The app will fail on the first request if either setting is missing.

## Running
With the defaults in `appsettings.json` and Azurite running locally:

```bash
dotnet run --project AzureStaticSiteHoster
```

Then browse to http://localhost:5103.

You can override the settings on the command line:

```bash
dotnet run --project AzureStaticSiteHoster AzureStorageConnectionString="ConnectionString" AzureStorageContainer="container"
```

Or with environment variables:

```bash
export AzureStorageConnectionString="ConnectionString"
export AzureStorageContainer="container"
dotnet run --project AzureStaticSiteHoster
```

### Quick start with Azurite
```bash
# Start the blob service of Azurite
docker run -d -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0

# Create a container and upload a site (using the Azure CLI)
az storage container create --name web --connection-string "UseDevelopmentStorage=true"
az storage blob upload-batch --destination web --source ./my-site --connection-string "UseDevelopmentStorage=true"

# Serve it
dotnet run --project AzureStaticSiteHoster
```

## Running in Docker
The image is built from the repository root:

```bash
docker build -f AzureStaticSiteHoster/Dockerfile -t azurestaticsitehoster .
```

The .NET 10 base image listens on port **8080** and runs as a non-root user. `UseDevelopmentStorage=true` points at `127.0.0.1`, which inside a container is the container itself, so when using an emulator on your host pass a full connection string pointing at `host.docker.internal`:

```bash
docker run --rm -p 8080:8080 \
  --add-host=host.docker.internal:host-gateway \
  -e AzureStorageConnectionString="DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://host.docker.internal:10000/devstoreaccount1;" \
  -e AzureStorageContainer="web" \
  azurestaticsitehoster
```

Then browse to http://localhost:8080. (The account key above is the well-known public Azurite development key, not a secret.)
