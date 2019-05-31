const {expect} = require('chai');
const axios = require('axios');
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
const timeout = 20000;
const appUrl = `https://${customHostName}`;
const optionDictionary = {
  'Strongly Agree': 'selected_answer-1',
  'Agree': 'selected_answer-2',
  'Disagree': 'selected_answer-3',
  'Strongly Disagree': 'selected_answer-4',
  "This doesn't apply to me": 'selected_answer-5'
};

parallel('Understand Myself cross-browser tests ', function() {
  this.timeout(1200000); // 20 mins
  for (let cap of capabilities) {
    it(`${cap.browserName}, ${cap.os? cap.os: cap.device}: Run through assessment, negative tests and check resume functionality`, async (done) => {
      const driver = await buildDriver(cap);


      console.log(`${cap.browserName}: Checking for error when clicking next without selecting any options`);
      let errorTitleText = '';
      await driver.get(appUrl);
      let startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), timeout);
      await startAssessmentButton.click();
      await driver.wait(until.urlContains(`q/short/01`), timeout);
      let nextButton = await driver.wait(until.elementLocated(By.className('govuk-button')), timeout);
      await nextButton.click();
      const errorTitle = await driver.wait(until.elementLocated(By.id('error-summary-title')), timeout);
      try {
        errorTitleText = await errorTitle.getText();
      }
      catch(err) {
        if (err.name === 'StaleElementReferenceError') errorTitleText = errorTitle.getText();
      }
      expect(errorTitleText.trim()).to.equal('There is a problem');

      console.log(`${cap.browserName}: Start assessment, get session ID and check if it resumes assessment`);
      await driver.get(appUrl);
      startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), timeout);
      await startAssessmentButton.click();
      const agreeOption = await driver.wait(until.elementLocated(By.id(optionDictionary['Agree'])), timeout);
      await agreeOption.click();
      await driver.findElement(By.className('govuk-button')).click();
      // Wait for page to load and click Save my progress
      await driver.wait(until.urlContains('q/short/02'), timeout);
      const saveProgressLink = await driver.wait(until.elementLocated(By.linkText('Save my progress')), timeout);
      try {
        await saveProgressLink.click();
      }
      catch(err) {
        if (err.name === 'StaleElementReferenceError') await saveProgressLink.click();
      }
      // Wait for page to load and save session ID text
      await driver.wait(until.urlContains('save-my-progress'), timeout);
      nextButton = await driver.wait(until.elementLocated(By.className('govuk-button')), timeout);
      await nextButton.click();
      const errorMessageElement = await driver.wait(until.elementLocated(By.className('govuk-error-summary__title')), timeout);
      const errorMessageText = await errorMessageElement.getText();
      expect(errorMessageText.trim()).to.equal('There is a problem');

      await driver.wait(() => selectAnswer(driver, 'SelectedOption-2'), timeout);
      await driver.findElement(By.className('govuk-button')).click();
      const sessionIdTextElement = await driver.wait(until.elementLocated(By.className('app-your-reference__code')), timeout);
      const sessionIdText = await sessionIdTextElement.getText();

      // Go to landing page, wait for page to load, enter session ID and click resume progress
      await driver.get(appUrl);
      const resumeEntryTextBox = await driver.wait(until.elementLocated(By.id('code')), timeout);
      await resumeEntryTextBox.sendKeys(sessionIdText);
      await driver.findElement(By.className('app-button')).click();
      //Wait for Url to contain q/2
      const urlFound = await driver.wait(until.urlContains('q/short/02'));
      expect(urlFound).to.equal(true);

      console.log(`${cap.browserName}: Check for error message if no session ID is entered`);
      await driver.get(appUrl);
      const resumeButton = await driver.wait(until.elementLocated(By.className('app-button')), timeout);
      await resumeButton.click();
      const refCodeValidation = await driver.wait(until.elementLocated(By.className('govuk-error-message')), timeout);
      errorText = await refCodeValidation.getText();
      expect(errorText.trim()).to.equal('Please enter your reference');

      console.log(`${cap.browserName}: Running through assessment`);
      await driver.get(appUrl);
      startAssessmentButton = await driver.wait(until.elementLocated(By.className('govuk-button--start')), timeout);
      await startAssessmentButton.click();

      var i = 1;
      var foundLastPage = false;
      var headers = {};

      // Get session ID, used for the axios request
      await driver.manage().getCookie('.dysac-session')
        .then(function(cookies){
          headers = {
            headers: {
              'Cookie': '.dysac-session='+cookies.value
            }
          }
          done()
        })

      while (!foundLastPage) {

        var url = `${appUrl}/q/short/${i < 10? 0: ''}${i}`;

        // Do a quick http request and check for 200 response,
        // set flag if we have reached the last page
        await axios.get(url, headers)
          .catch(() => {
            foundLastPage = true
            done()
          })

        // Load question page, click option and submit
        if (!foundLastPage) {
          await driver.wait(until.urlContains(`q/short/${i < 10? 0: ''}${i}`), timeout);
          await driver.wait(() => selectAnswer(driver, optionDictionary['Agree']), timeout);
          await driver.findElement(By.className('govuk-button')).click();
          i += 1;
        }

      }

      await driver.wait(until.urlContains('finish'), timeout);
      let seeResultsButton = await driver.wait(until.elementLocated(By.className('govuk-button')), timeout);
      await seeResultsButton.click();
      await driver.wait(until.urlContains('results'), timeout);
      const [firstElement, secondElement] = await driver.wait(until.elementsLocated(By.className('govuk-heading-l')), timeout);
      const resultText = await secondElement.getText();
      const numberOfResults = parseInt(resultText.split(' ')[0]);
      expect(numberOfResults).to.be.greaterThan(0);

      console.log(`${cap.browserName}: Run through filtering questions`);
      await driver.findElement(By.className('app-button')).click();
      let yesRadioButton = await driver.wait(until.elementLocated(By.id('selected_answer-1')), timeout);
      await yesRadioButton.click();
      await driver.findElement(By.className('govuk-button')).click();
      await driver.wait(until.urlContains('02'), timeout);
      yesRadioButton = await driver.wait(until.elementLocated(By.id('selected_answer-1')), timeout);
      await yesRadioButton.click();
      await driver.findElement(By.className('govuk-button')).click();
      await driver.wait(until.urlContains('finish'), timeout);
      seeResultsButton = await driver.wait(until.elementLocated(By.className('govuk-button')), timeout);
      await seeResultsButton.click();
      const lightText = await driver.wait(until.elementsLocated(By.className('text-light')), timeout);
      const filteredNumOfRolesText = await lightText[1].getText();
      const filteredNumOfRoles = parseInt(filteredNumOfRolesText.split(' ')[0]);
      expect(filteredNumOfRoles).to.be.greaterThan(0);
      await driver.quit();
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
