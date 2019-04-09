const {expect} = require('chai');
const {Builder, By, until} = require('selenium-webdriver');
const capabilities = require('../conf/conf').capabilities;
const parallel = require('mocha.parallel');

const customHostName = process.env.CustomHostName? process.env.CustomHostName : require('../Config/config').CustomHostName;

const buildDriver = (caps) => {
  return new Promise((resolve, reject) => {
    const driver = new Builder()
      .usingServer('https://hub-cloud.browserstack.com/wd/hub')
      .withCapabilities(caps)
      .build();
    resolve(driver);
  })
}

const appUrl = `https://${customHostName}`;
const optionDictionary = {
  'Strongly Agree': 'selected_answer-1',
  'Agree': 'selected_answer-2',
  'Disagree': 'selected_answer-3',
  'Strongly Disagree': 'selected_answer-4',
  "This doesn't apply to me": 'selected_answer-5'
};

parallel('Understand Myself tests ', function() {
  this.timeout(300000);
  for (let cap of capabilities) {
    it(`${cap.browserName}, ${cap.os? cap.os: cap.device}: Run through assessment, negative tests and check resume functionality`, async (done) => {

        const driver = await buildDriver(cap);

        console.log(`${cap.browserName}: Running through assessment`);
        await driver.get(appUrl);
        let startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), 20000);
        await startAssessmentButton.click();
    
        for (let i = 1; i < 41; i++) {
          await driver.wait(until.urlContains(`q/${i}`), 20000);
          await driver.wait(() => selectAnswer(driver, optionDictionary['Agree']), 20000);
          await driver.findElement(By.className('govuk-button')).click();
        }
    
        await driver.wait(until.urlContains('finish'), 20000);
        const seeResultsButton = await driver.wait(until.elementLocated(By.className('govuk-button')), 20000);
        await seeResultsButton.click();
        await driver.wait(until.urlContains('results'), 20000);
        const [firstElement, secondElement] = await driver.wait(until.elementsLocated(By.className('govuk-heading-l')), 20000);
        const resultText = await secondElement.getText();
        const numberOfResults = parseInt(resultText.split(' ')[0]);
        expect(numberOfResults).to.be.greaterThan(0);

        console.log(`${cap.browserName}: Checking for error when clicking next without selecting any options`);
        let errorText = '';
        await driver.get(appUrl);
        startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), 20000);
        await startAssessmentButton.click();
        await driver.wait(until.urlContains(`q/1`), 20000);
        const nextButton = await driver.wait(until.elementLocated(By.className('govuk-button')), 20000);
        await nextButton.click();
        let errorElement = await driver.wait(until.elementLocated(By.className('govuk-error-message')), 20000);
        try {
          errorText = await errorElement.getText();
        }
        catch(err) {
          if (err.name === 'StaleElementReferenceError') errorText = errorElement.getText();
        }
        expect(errorText.trim()).to.equal('Please select an option below to continue');

        console.log(`${cap.browserName}: Start assessment, get session ID and check if it resumes assessment`);
        await driver.get(appUrl);
        startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), 20000);
        await startAssessmentButton.click();
        const agreeOption = await driver.wait(until.elementLocated(By.id(optionDictionary['Agree'])), 20000);
        await agreeOption.click();
        await driver.findElement(By.className('govuk-button')).click();
        // Wait for page to load and click Save my progress
        await driver.wait(until.urlContains('q/2'), 20000);
        const saveProgressLink = await driver.wait(until.elementLocated(By.linkText('Save my progress')), 20000);
        try {
          await saveProgressLink.click();
        }
        catch(err) {
          if (err.name === 'StaleElementReferenceError') await saveProgressLink.click();
        }
        // Wait for page to load and save session ID text
        await driver.wait(until.urlContains('save-my-progress'), 20000);
        await driver.wait(() => selectAnswer(driver, 'SelectedOption-3'), 20000);
        await driver.findElement(By.className('govuk-button')).click();
        const sessionIdTextElement = await driver.wait(until.elementLocated(By.className('app-your-reference__code')), 20000);
        const sessionIdText = await sessionIdTextElement.getText();
    
        // Go to landing page, wait for page to load, enter session ID and click resume progress
        await driver.get(appUrl);
        const resumeEntryTextBox = await driver.wait(until.elementLocated(By.id('code')), 20000);
        await resumeEntryTextBox.sendKeys(sessionIdText);
        await driver.findElement(By.className('app-button')).click();
        //Wait for Url to contain q/2
        const urlFound = await driver.wait(until.urlContains('q/2'));
        expect(urlFound).to.equal(true);

        console.log(`${cap.browserName}: Check for error message if no session ID is entered`)
        await driver.get(appUrl);
        const resumeButton = await driver.wait(until.elementLocated(By.className('app-button')), 20000);
        await resumeButton.click();
        errorElement = await driver.wait(until.elementLocated(By.className('govuk-error-message')), 20000);
        errorText = await errorElement.getText();
        await driver.quit();
        expect(errorText.trim()).to.equal('The code could not be found');
        done();
      });
  }
});

async function selectAnswer(driver, option) {
  try {
    await driver.findElement(By.id(option)).click();
  }
  catch(err) {
    if (err.name === 'StaleElementReferenceError') await driver.findElement(By.id(option)).click();
  }
  const optionSelected = await driver.findElement(By.id(option)).isSelected();
  return optionSelected;
}