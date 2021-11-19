# AzureStaticSiteHoster
A small web application to make static websites in local azure storage emulators much better to use

## Why does this exist
Well, I was building what is essentially a giant static site generator that uses Azure Blob Storage to store the output
and to test things locally using the Azure storage emulator and Azurite I needed a way of viewing the results through a web browser. After many months of downloading from storage and then feeding it through another http server I built this. 

And hey, working offline on an airplane using just the local emulator is pretty cool too.

## Building
A simple `dotnet build` or just use an IDE like Visual Studio or Rider

## Running
Before running this you'll need to set two variables in the appsettings.json
1. AzureStorageConnectionString: Your connection string to your Azure Storage (For the local emulator just use "UseDevelopmentStorage=True")
2. AzureStorageContainer The container to use as the source

You could also set them in your dotnet run command by doing `dotnet run AzureStorageConnectionString="ConnectionString" AzureStorageContainer="container"`

