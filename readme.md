# Discover Skills and Careers

## Architecture Documents 

[High level solution Architecture Diagram](https://drive.google.com/open?id=16ukfuSa6eKJW3lgVBvaFFobpEQ3a13D2)


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
