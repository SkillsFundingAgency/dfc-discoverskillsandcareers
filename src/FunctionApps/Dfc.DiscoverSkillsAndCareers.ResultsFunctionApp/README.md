# Results Function Application

This function application provides an API which given a user session Id will compute the selected job profiles. The job profiles come from an Azure Search Index provided by explore careers.

## Configuration

    {
        "CosmosSettings": {
            "Endpoint": "",
            "Key": "",
            "DatabaseName": ""
        },
        "AppSettings": {
            "SessionSalt": "",
            "NotifyApiKey": ""
        },
        "AzureSearchSettings": {
          "ServiceName": "",
          "IndexName": "",
          "ApiKey": ""
        }
    }

#### CosmosSettings Section

* **Endpoint** - The endpoint URI for the Cosmos DB instance
* **Key** - The access key for the Cosmos DB instance
* **DatabaseName** - The database name to be used in the given cosmos instance

#### AppSettings Section

* **SessionSalt** - Used to generate session keys (if required), should be consistent across all function applications within a given environment
* **NotifyApiKey** - The api key used to call the [GOV.UK Notify service](https://docs.notifications.service.gov.uk/net.html#net-client-documentation) to send e-mails and SMS 

#### Azure Search Section 

* **ServiceName** - The name of the Azure Search service to connect to.
* **IndexName** - The name of the index within the Azure service to connect to
* **ApiKey** - The api key to use to connect to the Azure Search index.