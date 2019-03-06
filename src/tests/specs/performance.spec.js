const {expect} = require('chai');
const puppeteer = require('puppeteer');
const lighthouse = require('lighthouse');

const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};

const appUrl = 'https://discover-skills-careers-dev.nationalcareersservice.org.uk';

describe('Lighthouse performance testing for Understand Myself - National Careers Service', function() {
    this.timeout(180000);
    const opts = {
        port: 9222,
        onlyCategories: ['performance'],
        output: 'json'
    };
    const performanceThreshold = 0.9;

    it('Home page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then(browser => {
            return browser.newPage().then(page => {
                return page.goto(appUrl)
                    .then(() => lighthouse(page.url(), opts, null))
                    .then(results => {
                        browser.close();
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });
    });
    
    it('Start page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then(browser => {
            return browser.newPage().then(page => {
                return page.goto(`${appUrl}/start`)
                    .then(() => lighthouse(page.url(), opts, null))
                    .then(results => {
                        browser.close();
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });
    });

    it('Statement page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then(browser => {
            return browser.newPage().then(page => {
                return page.goto(`${appUrl}/q/1?assessmentType=short`)
                    .then(() => lighthouse(page.url(), opts, null))
                    .then(results => {
                        browser.close();
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });
    });

    it('Save progress page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then((browser) => {
            return browser.newPage().then((page) => {
                // TODO: change url to dev env once known
                return page.goto(`${appUrl}/q/1?assessmentType=short`)
                    .then(() => Promise.all([page.waitForNavigation(), page.click('.govuk-link--no-visited-state')]))
                    .then(([response]) => lighthouse(response.url(), opts, null))
                    .then(results => {
                        browser.close();
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });        
    });

    it('Finish page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then((browser) => {
            return browser.newPage().then((page) => {
                // TODO: change url to dev env once known
                return page.goto(`${appUrl}/q/1?assessmentType=short`)
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
                        browser.close();
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });
    });

    it('Results page', () => {
        return puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]}).then((browser) => {
            return browser.newPage().then((page) => {
                // TODO: change url to dev env once known
                return page.goto(`${appUrl}/q/1?assessmentType=short`)
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
                        browser.close();                              
                        expect(results.lhr.categories.performance.score).to.be.at.least(performanceThreshold);
                    });
            });
        });
    });
});