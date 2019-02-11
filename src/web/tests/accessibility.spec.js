const pa11y = require('pa11y');
const {expect} = require('chai');
const fs = require('fs');

describe('Accessibility testing for web pages', function () {
    this.timeout(5000);
    // Create index of html pages
    const htmlPages = fs.readdirSync('./src/templates');
    let resultsJSON = JSON.parse(fs.readFileSync('./tests/log/results.json'));

    // Pa11y test each html page
    htmlPages.forEach(page => {
        it(page, () => {
            return pa11y(`http://localhost:3000/${page}`, {
                standard: "WCAG2AA",
                ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
            })
                .then(({issues}) => {
                    const issuesWithPageName = issues.map(issue => {
                        issue.page = page;
                        return issue
                    });
                    resultsJSON.release.pa11y = resultsJSON.release.pa11y.concat(issuesWithPageName);
                    fs.writeFileSync('./tests/log/results.json', JSON.stringify(resultsJSON));
                    expect(issues).to.eql([]);
                });
        });
    });
});