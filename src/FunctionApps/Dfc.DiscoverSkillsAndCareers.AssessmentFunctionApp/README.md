# Assessment Function Application


This function application provides an API which defines the overall assessment workflow used by discover you skills and careers front end.

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