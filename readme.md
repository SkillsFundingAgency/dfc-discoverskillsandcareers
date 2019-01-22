# Discover Skills and Careers

## Architecture Documents 

[High level solution Architecture Diagram](https://drive.google.com/open?id=16ukfuSa6eKJW3lgVBvaFFobpEQ3a13D2)

## Developer Requirements

* Visual Studio 2017 / Visual Studio Code
* .NET Core 2.1 or higher
* [Azure Functions Tools](https://www.npmjs.com/package/azure-functions-core-tools)
* [Azure Storage Emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator) - Note, this is included as part of the Azure SDK but a standalone installer is available for windows. Alternatively you could create a storage account on azure.
* [Azure Cosmos Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)

## Solution Structure

* **Dfc.DiscoverSkillsAndCareers.FunctionApp** - Contains the code for the function application deployables 
* **Dfc.DiscoverSkillsAndCareers.Models** - Any shared code that may be required.
* **Dfc.DiscoverSkillsAndCareers.Repositories** - Any data access code that is required 
* **web** - static assests to be deployed to blob service. 

## Building Solution 

### Building and Running Function App

Create a local.settings.json file (change as requried but the following works with the Cosmos and Blob emulators)
```
{
    "CosmosSettings": {
        "Endpoint": "https://localhost:8081",
        "Key": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        "DatabaseName": "TestDatabase"
    },
    "BlobStorage": {
        "StorageConnectionString": "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;",
        "ContainerName": "mycontainer"
    },
    "StaticSiteDomain": "https://dfc-dev-skillscareers-as.azurewebsites.net"
}
```
To build the function app navigate to `src/Dfc.DiscoverSkillsAndCareers.FunctionApp` and run 

    dotnet build 

to run the function app again 

    func host start

### Build web assets and templates 

#### Development 

To build the web assets and templates navigate to `src/web` and run 

    gulp dev

#### Production 

To build the web assets and templates navigate to `src/web` and run 

    gulp

this will build the assets to `src/web/dist`

Production task also revisions assets. 

#### Asset Revisioning 

Assets are revisioned using [`gulp-rev`](https://github.com/sindresorhus/gulp-rev) and references to those files are updated using [`gulp-rev-rewrite`](https://github.com/TheDancingCode/gulp-rev-rewrite). 

Note that these assets are only revisioned when the file is changed, and not every time the task is run. 

### Front-end Testing 

@todo

#### Linting 

@todo

#### Cross-browser

@todo

#### Accessibility

@todo


## Deployment Structure 

There are 3 deployment artifacts 

1. Function App - To be deployed to the function environment.
2. Page templates - To be deployed to blob container.
3. Static Landing Pages - To be deployed to blob container. 
