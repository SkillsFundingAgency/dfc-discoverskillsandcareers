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

## Deployment Structure 

There are 3 deployment artifacts 

1. Function App - To be deployed to the function environment.
2. Page templates - To be deployed to blob container (/templates).
3. Static Landing Pages - To be deployed to blob container (/$web). 
