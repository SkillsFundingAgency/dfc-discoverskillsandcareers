# Discover Skills and Careers

## Screenshots 

![](screenshot.png) 

## Live examples

### DEV

- Web: https://dfcdevskillscareersstr.z6.web.core.windows.net
- Questions: https://dfc-dev-skillscareers-fa.azurewebsites.net/q/1?assessmentType=short

### SIT

- Web: https://dfcsitskillscareersstr.z6.web.core.windows.net
- Questons: https://dfc-sit-skillscareers-fa.azurewebsites.net/q/1?assessmentType=short


## Technical documentation  

### Architecture Documents 

![](Architecture-latest.png) 

### Dependencies

* Visual Studio 2017 / Visual Studio Code
* .NET Core 2.1 or higher
* [Azure Functions Tools](https://www.npmjs.com/package/azure-functions-core-tools)
* [Azure Cosmos Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
* NodeJS 10.15.3 or higher

### Solution Structure

* **Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp** - Assessment apis for new, move, answer, reload
* **Dfc.DiscoverSkillsAndCareers.CmsFunctionApp** - CMS timer function 
* **Dfc.DiscoverSkillsAndCareers.ContentFunctionApp** - Content apis to supply CMS content to pages
* **Dfc.DiscoverSkillsAndCareers.QuestionsFunctionApp** - Question apis to expose question data
* **Dfc.DiscoverSkillsAndCareers.ResultFunctionApp** - Results api to fetch results for a statement
* **Dfc.DiscoverSkillsAndCareers.Models** - Any data models.
* **Dfc.DiscoverSkillsAndCareers.Repositories** - Any data access code that is required 
* **Dfc.DiscoverSkillsAndCareers.WebApp** - MCV app
* **tests** - Front end tests.

### Running the application

#### Building and Running the Api Function Apps

Create a local.settings.json file (change as requried but the following works with the Cosmos emulator)
```
{
    "CosmosSettings": {
        "Endpoint": "https://localhost:8081",
        "Key": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        "DatabaseName": "TestDatabase"
    },
    "AppSettings": {
        "SessionSalt": "ncs"
    }
}
```
To build an api function app navigate to `src/Dfc.DiscoverSkillsAndCareers.AssessmentFunctionApp` and run 

    dotnet build 

to run the function app again 

    func host start

repeat for all apis to run locally.

#### Building and running the front-end 

To build an api function app navigate to `src/Dfc.DiscoverSkillsAndCareers.WebApp` and run 

    dotnet run 

#### Visual Studio run everything for debug

You can set your startup projects to start all, the apis and the front-end at the same time to debug.

![](screenshot-startprojects.png) 

#### Asset Revisioning 

Assets are revisioned using [`gulp-rev`](https://github.com/sindresorhus/gulp-rev) and references to those files are updated using [`gulp-rev-rewrite`](https://github.com/TheDancingCode/gulp-rev-rewrite). 

Note that these assets are only revisioned when the file is changed, and not every time the task is run. 

#### JavaScript 

JavaScript is written in ES6 and is compiled using Babel in order to support older browsers. 

JavaScript is linted using the [Standard](https://standardjs.com) linter, as documented in the [GOV.UK coding standards and guidelines](https://github.com/alphagov/styleguides/blob/master/js.md#linting)

#### Linting 

Sass linting config is taken from the [`gov-lint`](https://github.com/alphagov/govuk-lint/blob/master/configs/scss_lint/gds-sass-styleguide.yml) project and [converted from the “SCSS Lint” style to “Sass Lint”](http://sasstools.github.io/make-sass-lint-config/) to work with the [`sass-lint`](https://www.npmjs.com/package/sass-lint) module.

### Running the test suite

#### Front-end

    cd src/tests
    npm test 

#### Cross-browser Testing

Cross-browser testing is carried out using [BrowserStack](https://www.browserstack.com/automate/protractor). BrowserStack testing will run through positive/negative paths. To run manually navigate to `src/tests` and run

    protractor conf/conf.js 

#### Accessibility Testing

Accessibility testing is carried out using [Pa11y](https://github.com/pa11y/pa11y) with WCAG2AA used as testing standard. To run manually navigate to `src/tests` and run

    mocha specs/accessibility.spec.js

#### Performance Testing

Performance testing is carried out using [Lighthouse](https://github.com/GoogleChrome/lighthouse#readme). To run manually navigate to `src/tests` and run

    mocha specs/performance.spec.js

## Deployment Structure 

There are 6 deployment artifacts 

1. **Assessment Api Function** - To be deployed to the api function environment.
2. **CMS Api Function** - To be deployed to the api function environment.
3. **Content Api Function** - To be deployed to the api function environment.
4. **Questions Api Function** - To be deployed to the api function environment.
5. **Resukts Api Function** - To be deployed to the api function environment.
6. **MVC Web App** - To be deployed to the web environment.

The support app can be run in order to create the relevant Statement and Content data.

## Analytics 

### Event Tracking

HTML Elements

    gov-analytics-data="{{pageName}} | click | button | Start assessment"

Nunjucks partials

    {{ govukButton({
        text: "Resume progress",
        classes: "app-button",
        attributes: {
          "gov-analytics-data": pageName + " | click | button | Resume progress"
        }
      }) }}

## Licence

TODO