const {expect} = require('chai');
const puppeteer = require('puppeteer');
const lighthouse = require('lighthouse');
const customHostName = process.env.CustomHostName? process.env.CustomHostName : require('../Config/config').CustomHostName;

const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};

const appUrl = `https://${customHostName}`;

describe('Lighthouse performance testing for Understand Myself - National Careers Service', function() {
    this.timeout(180000);
    const opts = {
        port: 9222,
        onlyCategories: ['performance'],
        output: 'json'
    };
    const performanceThreshold = 0.9;

    it('Home page', async () => {
        const browser = await puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]});
        console.log('Launching Headless Chrome')
        const page = await browser.newPage();
        console.log(`Going to URL: ${appUrl}`)
        await page.goto(appUrl);
        console.log(`Performing performance test on ${page.url()}`)
        const {lhr: {categories}} = await lighthouse(page.url(), opts, null);
        console.log('Closing session')
        await browser.close();
        expect(categories.performance.score).to.be.at.least(performanceThreshold);
    });

    it('Statement page', async () => {
        const browser = await puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]});
        console.log('Launching Headless Chrome')
        const page = await browser.newPage();
        console.log(`Going to URL: ${appUrl}`)
        await page.goto(appUrl);
        console.log('Clicking button with selector .govuk-button--start')
        const [response] = await Promise.all([page.waitForNavigation(), page.click('.govuk-button--start')]);
        console.log(`Navigation successfull: ${response.ok()}. Performing performance test on ${response.url()}`)
        const {lhr: {categories}} = await lighthouse(response.url(), opts, null);
        console.log('Closing session')
        await browser.close();
        expect(categories.performance.score).to.be.at.least(performanceThreshold);
    });

    it('Save progress page', async () => {
        const browser = await puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]});
        console.log('Launching Headless Chrome')
        const page = await browser.newPage();
        console.log(`Going to URL: ${appUrl}`)
        await page.goto(appUrl);
        console.log('Clicking button with selector .govuk-button--start')
        await Promise.all([page.waitForNavigation(), page.click('.govuk-button--start')]);
        console.log('Clicking button with selector .govuk-link--no-visited-state')
        const [response] = await Promise.all([page.waitForNavigation(), page.click('.govuk-link--no-visited-state')]);
        console.log(`Navigation successfull: ${response.ok()}. Performing performance test on ${response.url()}`)
        const {lhr: {categories}} = await lighthouse(response.url(), opts, null);
        console.log('Closing session')
        await browser.close();
        expect(categories.performance.score).to.be.at.least(performanceThreshold);
    });

    it('Finish page', async () => {
        const browser = await puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]});
        const page = await browser.newPage();
        await page.goto(appUrl);
        await Promise.all([page.waitForNavigation(), page.click('.govuk-button--start')]);
        for (let i = 0; i < 39; i++) {
            await page.click(`#${answerDict['Agree']}`);
            await Promise.all([page.waitForNavigation(), page.click('.govuk-button')]);
        }
        await page.click(`#${answerDict['Agree']}`);
        const [response] = await Promise.all([page.waitForNavigation(), page.click('.govuk-button')]);
        const {lhr: {categories}} = await lighthouse(response.url(), opts, null);
        await browser.close();
        expect(categories.performance.score).to.be.at.least(performanceThreshold);
    });

    it('Results page', async () => {
        const browser = await puppeteer.launch({args: [`--remote-debugging-port=${opts.port}`]});
        const page = await browser.newPage();
        await page.goto(appUrl);
        await Promise.all([page.waitForNavigation(), page.click('.govuk-button--start')]);
        for (let i = 0; i < 40; i++) {
            await page.click(`#${answerDict['Agree']}`);
            await Promise.all([page.waitForNavigation(), page.click('.govuk-button')]);
        }
        const [response] = await Promise.all([page.waitForNavigation(), page.click('.govuk-button')]);
        const {lhr: {categories}} = await lighthouse(response.url(), opts, null);
        await browser.close();
        expect(categories.performance.score).to.be.at.least(performanceThreshold);
    });
});