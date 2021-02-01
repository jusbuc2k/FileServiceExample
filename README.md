# File Service API Reference Project

This is a simple HTTP REST API that supports:

- Uploading files
- Downloading files
- Listing files
- Deleting files

This service does not implement any authentication mechanism.

# PreRequisites

.NET 5 SDK

https://dotnet.microsoft.com/download

# Running

From the CLI, in the src/FileServiceAPI folder
```
dotnet run
```
Or debug in Visual Studio or VS Code

# Notes

File metadata is handled by a class that implements ```IFileMetadataService``` interface, while file contents themeslves are handled
by a class that implements ```IFileBlobService```. This was done so that file information could be persisted in something like a database,
object store, etc, while the file contents would be persisted in something like a file system, blob store, etc.

The default implementation uses the ```InMemoryFileService```, which, as it sounds, just stores the file metadata and contents in memory,
for the life of the hosting process. An example implementation for using an Azure Blob storage for both metadata and content is provided 
as well in the form of ```AzureBlobFileService```.

This service is naive, in that it doesn't deal with things like "On delete, what if the file metadata gets deleted but the file contents doesn't".

# Build

Run the ```build.ps1``` for a release & publish capable build. This script also executes the Nerdbank.GitVersioning CLI for use with 
a cloud build agent.