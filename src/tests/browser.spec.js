const protractor = require('protractor');

const appUrl = 'https://discover-skills-careers-dev.nationalcareersservice.org.uk';
const EC = protractor.ExpectedConditions;
const optionDictionary = {
  'Strongly Agree': 'selected_answer-1',
  'Agree': 'selected_answer-2',
  'Disagree': 'selected_answer-3',
  'Strongly Disagree': 'selected_answer-4',
  "This doesn't apply to me": 'selected_answer-5'
};

describe('National Careers Service', () => {
  browser.ignoreSynchronization = true;
  it('Run through short assessment and get result', async () => {
    // Go to landing page
    // TODO: change url to dev env once known
    await browser.driver.get(appUrl);
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-link--no-visited-state'))), 10000);
    await browser.driver.findElement(By.className('govuk-link--no-visited-state')).click();
    await browser.driver.wait(EC.urlContains('start'), 10000);
    await browser.driver.wait(EC.presenceOf(element(by.linkText('Start now'))), 10000);
    await browser.driver.findElement(By.linkText('Start now')).click();

    for (let i = 1; i < 41; i++) {
      await browser.driver.wait(EC.urlContains(`q/${i}`), 10000);
      await browser.driver.wait(() => selectAnswer(optionDictionary['Agree']), 10000);
      await browser.driver.findElement(By.className('govuk-button')).click();
    }

    await browser.driver.wait(EC.urlContains('finish'), 10000);
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-button'))), 10000);
    await browser.driver.findElement(By.className('govuk-button')).click();
    await browser.driver.wait(EC.urlContains('results'), 10000);
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-body-l'))), 10000);
    const numberOfResultsStr = await browser.driver.findElement(By.className('govuk-body-l')).getText();
    const numberOfResults = parseInt(numberOfResultsStr.split(' ')[0]);
    expect(numberOfResults > 0).toEqual(true);
  }, 180000);

  it('Get error message when clicking continue without selecting an option', async () => {
    await browser.driver.get(`${appUrl}/q/1?assessmentType=short`);
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-button'))), 10000);
    await browser.driver.findElement(By.className('govuk-button')).click();
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-error-message'))), 10000);
    const errorText = await browser.driver.findElement(By.className('govuk-error-message')).getText();
    expect(errorText.trim()).toEqual('Please select an option above or this does not apply to continue');
  }, 30000);

  it('Start assessment, get session ID and check if it resumes assessment', async () => {
    await browser.driver.get(`${appUrl}/q/1?assessmentType=short`);
    await browser.driver.wait(EC.presenceOf(element(by.id(optionDictionary['Agree']))), 10000);
    await browser.driver.findElement(By.id(optionDictionary['Agree'])).click();
    await browser.driver.findElement(By.className('govuk-button')).click();
    // Wait for page to load and click Save my progress
    await browser.driver.wait(EC.urlContains('q/2'), 5000);
    await browser.driver.wait(EC.presenceOf(element(by.linkText('Save my progress'))), 5000);
    await browser.driver.findElement(By.linkText('Save my progress')).click();
    // Wait for page to load and save session ID text
    await browser.driver.wait(EC.urlContains('save-my-progress'), 5000);
    await browser.driver.wait(EC.presenceOf(element(by.className('app-your-reference__code'))), 5000);
    const sessionIdText = await browser.driver.findElement(By.className('app-your-reference__code')).getText();

    // Go to landing page, wait for page to load, enter session ID and click resume progress
    await browser.driver.get(appUrl)
    await browser.driver.wait(EC.presenceOf(element(by.id('code'))), 10000);
    await browser.driver.findElement(By.id('code')).sendKeys(sessionIdText);
    await browser.driver.findElement(By.className('govuk-button')).click();
    //Wait for Url to contain q/2
    const urlFound = await browser.driver.wait(EC.urlContains('q/2'));
    expect(urlFound).toEqual(true);
  }, 60000); 
  
  it('Error message if no session ID is entered', async () => {
    await browser.driver.get(appUrl);
    await browser.driver.wait(EC.presenceOf(element(by.linkText('Resume progress'))), 10000);
    await browser.driver.findElement(By.linkText('Resume progress')).click();
    await browser.driver.wait(EC.presenceOf(element(by.className('govuk-error-message'))), 10000);
    const errorText = await browser.driver.findElement(By.className('govuk-error-message')).getText();
    expect(errorText.trim()).toEqual('Please enter a reference code');
  }, 30000);
});

async function selectAnswer(option) {
  await browser.driver.findElement(by.id(option)).click();
  const optionSelected = await browser.driver.findElement(by.id(option)).isSelected();
  return optionSelected;
}