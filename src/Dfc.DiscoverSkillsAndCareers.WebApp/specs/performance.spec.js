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
const performanceScores = [];

describe('Lighthouse performance testing for Understand Me web pages', function() {
    this.timeout(20000);
    const opts = {
        chromeFlags: ['--headless'],
        onlyCategories: ['performance'],
        output: 'json'
    };
    const performanceThreshold = 0.9;

    it('Home page', () => {
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/', opts, ).then(({categories}) => {
            performanceScores.push(addPageName(categories.performance, 'Home'));
            expect(categories.performance.score).to.be.at.least(performanceThreshold);
        });
    });
    
    it('Start page', () => {
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/start', opts, ).then(({categories}) => {
            performanceScores.push(addPageName(categories.performance, 'Start'));
            expect(categories.performance.score).to.be.at.least(performanceThreshold);
        });
    });

    it('Statement page', () => {
        return launchChromeAndRunLighthouse('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1', opts, ).then(({categories}) => {
            performanceScores.push(addPageName(categories.performance, 'Statement'));
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
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1')
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-link--no-visited-state')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then(results => {
                                instance.kill();
                                performanceScores.push(addPageName(results.lhr.categories.performance, 'Save Progress'));
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
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1')
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for second statement and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for third statement and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for fourth statement and click Finish
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then((results) => {
                                instance.kill();
                                performanceScores.push(addPageName(results.lhr.categories.performance, 'Finish'));
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
                        return page.goto('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1')
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for second statement and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for third statement and click next
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Select answer for fourth statement and click Finish
                            .then(() => page.click(`#${answerDict['Agree']}`))
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            // Click View Results
                            .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-button')]))
                            .then(([response]) => lighthouse(response.url(), opts, null))
                            .then((results) => {
                                instance.kill();
                                performanceScores.push(addPageName(results.lhr.categories.performance, 'Results'));
                                resultsJSON.release.lighthouse = performanceScores;
                                fs.writeFileSync('./log/results.json', JSON.stringify(resultsJSON));
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

function addPageName(performanceScore, pageName) {
    performanceScore.page = pageName;
    return performanceScore;
}