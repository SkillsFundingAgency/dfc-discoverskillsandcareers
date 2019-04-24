#CMS Function Application


This function application polls the given sitefinity instance for changes to Content, Questions and Question Sets and then updates the Discover Your skills and careers database appropriately. 

##Configuration

    {
      "CosmosSettings": {
            "Endpoint": "",
            "Key": "",
            "DatabaseName": ""
      },
      "AppSettings": {
        "SessionSalt": "",
        "SiteFinityApiUrlbase": "",
        "SiteFinityApiWebService": "",
        "SiteFinityClientId": "",
        "SiteFinityClientSecret": "",
        "SiteFinityUsername": "",
        "SiteFinityPassword": "",
        "SiteFinityRequiresAuthentication": "",
        "SiteFinityScope": "",
        "SiteFinityApiAuthenicationEndpoint": "",
        "SiteFinityJobCategoriesTaxonomyId": "" 
      },
      "Values": {
          "AzureWebJobsStorage": "",
          "PollingSchedule": ""
      }
    }

#### CosmosSettings Section

* **Endpoint** - The endpoint URI for the Cosmos DB instance
* **Key** - The access key for the Cosmos DB instance
* **DatabaseName** - The database name to be used in the given cosmos instance

#### AppSettings Section

* **SessionSalt** - Used to generate session keys (if required), should be consistent across all function applications within a given environment
* **SiteFinityApiUrlbase** - The URI location of the Sitefinity instance to extract content from
* **SiteFinityApiWebService** - The name of the webservice instance to call.
* **SiteFinityClientId** - The Client ID to use when calling the webservice (the `ClientId`, `ClientSecret` and  `Scope` should be setup as per  the [this documentation](https://www.progress.com/documentation/sitefinity-cms/request-access-token-for-calling-web-services)
* **SiteFinityClientSecret** - The client secret associated with the `Client Id`
* **SiteFinityUsername** - The Sitefinity username to log in as
* **SiteFinityPassword** - The password assocated with the user defined in _SiteFinityUsername_
* **SiteFinityRequiresAuthentication** - Defines whether to use a auth token handshake or to login to SiteFinity anonymously.
* **SiteFinityApiAuthenicationEndpoint** - The the URI (relative to _SiteFinityApiUrlbase_) that defines the authentication endpoint (typically `SiteFinity/Authenticate/openid/connect/token`)  
* **SiteFinityJobCategoriesTaxonomyId** - The name of the taxonomy that holds the Job Profile Category data (typically `Job Profile Categories`)

#### Values Section 

* **AzureWebJobsStorage** - The storage account to use for the function application 

* **PollingSchedule** - The cron expression which defines how frequently this function should run.



