# Content Function Application

This function application serves all of the content cached in the discover your skills and careers cosmos instance. This function does not actually get the content from the CMS. This is task is delegated to the [CMS Function Application](https://github.com/SkillsFundingAgency/dfc-discoverskillsandcareers/tree/develop/src/FunctionApps/Dfc.DiscoverSkillsAndCareers.CmsFunctionApp)

## Configuration

    {
        "CosmosSettings": {
            "Endpoint": "",
            "Key": "",
            "DatabaseName": ""
        },
        "AppSettings": {
            "SessionSalt": ""
        }
    }

#### CosmosSettings Section

* **Endpoint** - The endpoint URI for the Cosmos DB instance
* **Key** - The access key for the Cosmos DB instance
* **DatabaseName** - The database name to be used in the given cosmos instance

#### AppSettings Section

* **SessionSalt** - Used to generate session keys (if required), should be consistent across all function applications within a given environment


