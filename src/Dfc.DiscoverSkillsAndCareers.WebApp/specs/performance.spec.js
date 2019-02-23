const {expect} = require('chai');
const fs = require('fs');
const util = require('util');
const request = require('request');
const puppeteer = require('puppeteer');
const lighthouse = require('lighthouse');
const chrome = require('chrome-launcher');
const resultsJSON = require('../log/results')

const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};
const performanceScores = {
    home: {score: 0},
    start: {score: 0},
    statement: {score: 0},
    saveProgress: {score: 0},
    finish: {score: 0},
    results: {score: 0}
};

describe('Lighthouse performance testing for Understand Me web pages', function() {
    this.timeout(120000);
    const opts = {
        chromeFlags: ['--headless'],
        onlyCategories: ['performance'],
        output: 'json'
    };
    const performanceThreshold = 0.9;

    after(function() {
        resultsJSON.release.lighthouse = performanceScores;
        fs.writeFileSync('./log/results.json', JSON.stringify(resultsJSON));
    });

    it('Home page', () => {
        // TODO: change url to dev env once known
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/', opts, ).then(({categories}) => {
            performanceScores.home = categories.performance;
            expect(categories.performance.score).to.be.at.least(performanceThreshold);
        });
    });
    
    it('Start page', () => {
        // TODO: change url to dev env once known
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/start', opts, ).then(({categories}) => {
            performanceScores.start = categories.performance;
            expect(categories.performance.score).to.be.at.least(performanceThreshold);
        });
    });

    it('Statement page', () => {
        // TODO: change url to dev env once known
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1?assessmentType=short', opts, ).then(({categories}) => {
            performanceScores.statement = categories.performance;
            expect(categories.performance.score).to.be.at.least(performanceThreshold);
        });
    });

    it('Save progress page', () => {
        return chrome.launch({chromeFlags: opts.chromeFlags}).then(instance => {
            opts.port = instance.port;
            return util.promisify(request)(`http://localhost:${opts.port}/json/version`).then((resp) => {
                const {webSocketDebuggerUrl} = JSON.parse(resp.body);
                return puppeteer.connect({browserWSEndpoint: webSocketDebuggerUrl}).then((browser) => {
                    return browser.newPage().then((page) => {
                        // TODO: change url to dev env once known
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1?assessmentType=short')
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-link--no-visited-state')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then(results => {
                                instance.kill();
                                performanceScores.saveProgress = results.lhr.categories.performance;
                                expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                            });
                    });
                });
            });
        });
    });

    it('Finish page', () => {
        return chrome.launch({chromeFlags: opts.chromeFlags}).then(instance => {
            opts.port = instance.port;
            return util.promisify(request)(`http://localhost:${opts.port}/json/version`).then((resp) => {
                const {webSocketDebuggerUrl} = JSON.parse(resp.body);
                return puppeteer.connect({browserWSEndpoint: webSocketDebuggerUrl}).then((browser) => {
                    return browser.newPage().then((page) => {
                        // TODO: change url to dev env once known
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1?assessmentType=short')
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 2 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 3 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 4 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 5 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 6 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 7 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 8 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 9 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 10 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 11 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 12 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 13 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 14 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 15 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 16 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 17 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 18 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 19 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 20 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 21 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 22 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 23 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 24 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 25 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 26 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 27 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 28 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 29 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 30 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 31 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 32 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 33 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 34 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 35 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 36 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 37 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 38 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 39 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 40 and click Finish
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then((results) => {
                                instance.kill();
                                performanceScores.finish = results.lhr.categories.performance;
                                expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                            });
                    });
                });
            });
        });
    });

    it('Results page', () => {
        return chrome.launch({chromeFlags: opts.chromeFlags}).then(instance => {
            opts.port = instance.port;
            return util.promisify(request)(`http://localhost:${opts.port}/json/version`).then((resp) => {
                const {webSocketDebuggerUrl} = JSON.parse(resp.body);
                return puppeteer.connect({browserWSEndpoint: webSocketDebuggerUrl}).then((browser) => {
                    return browser.newPage().then((page) => {
                        // TODO: change url to dev env once known
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1?assessmentType=short')
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 2 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 3 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 4 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 5 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 6 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 7 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 8 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 9 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 10 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 11 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 12 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 13 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 14 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 15 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 16 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 17 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 18 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 19 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 20 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 21 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 22 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 23 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 24 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 25 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 26 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 27 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 28 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 29 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 30 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 31 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 32 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 33 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 34 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 35 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 36 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 37 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 38 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 39 and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for statement 40 and click Finish
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Click View Results
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then((results) => {
                                instance.kill();
                                performanceScores.results = results.lhr.categories.performance;                                
                                expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                            });
                    });
                });
            });
        });
    });
});

// launches chrome and performs a lighthouse test
function launchChromeAndRunLighthouse(url, opts, config = null) {
    return chrome.launch({chromeFlags: opts.chromeFlags}).then(instance => {
      opts.port = instance.port;
      return lighthouse(url, opts, config).then(results => {
        return instance.kill().then(() => results.lhr)
      });
    });
}