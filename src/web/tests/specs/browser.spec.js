describe('National Careers Service', () => {
  it('Run through short assessment and get result', () => {
    // Go to landing page
    browser.driver.get('https://dfcdevskillscareersstr.z6.web.core.windows.net/')
      // Wait for page to load and click Start assessment
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start assessment')).click())
      // Wait for page to load and click Start now
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start now')).click())
      // Wait for page to load, select Disagree for Q1 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q2 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q3 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q4 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q5 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q6 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q7 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select This doesn't apply to me for Q8 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.className('app-button--outline')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q9 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q10 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q11 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q12 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q13 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q14 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q15 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q16 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q17 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q18 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Disagree for Q19 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-4')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q20 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q21 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q22 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q23 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Disagree for Q24 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-4')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q25 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q26 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q27 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q28 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q29 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q30 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q31 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q32 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q33 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q34 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q35 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q36 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Strongly Agree for Q37 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-1')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q38 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Agree for Q39 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load, select Disagree for Q40 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-3')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      //Click View results
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      //Wait for page to load and get text
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.xpath('//h1')).getText())
      .then((text) => {
        expect(text).toEqual('Job categories you might be suited to');
      });
  }, 180000);

  it('Get error message when clicking continue without selecting an option', () => {
    // Go to landing page
    browser.driver.get('https://dfcdevskillscareersstr.z6.web.core.windows.net/')
      // Wait for page to load and click Start assessment
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start assessment')).click())
      // Wait for page to load and click Start now
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start now')).click())
      // Wait for page to load and Click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load and check for error message text
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.className('govuk-error-message')).getText())
      .then((errorText) => {
        expect(errorText.trim()).toEqual('Please select an option above or this does not apply to continue');
    });
  }, 30000);

  it('Start short assessment, get session ID and check if it resumes assessment', () => {
    let sessionId = null;
    // Go to landing page
    browser.driver.get('https://dfcdevskillscareersstr.z6.web.core.windows.net/')
      // Wait for page to load and Click Start assessment
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start assessment')).click())
      // Wait for page to load and click Start now
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Start now')).click())
      // Wait for page to load, Select Agree for Q1 and click Next
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('selected_answer-2')).click())
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      // Wait for page to load and click Save my progress
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.linkText('Save my progress')).click())
      // Wait for page to load and save session ID text
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.className('app-your-reference__code')).getText())
      .then((sessionIdText) => {
        sessionId = sessionIdText;
      });

    // Go to landing page, wait for page to load, enter session ID and click resume progress
    browser.driver.get('https://dfcdevskillscareersstr.z6.web.core.windows.net/')
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.findElement(By.id('code')).sendKeys(sessionId))
      .then(() => browser.driver.findElement(By.className('govuk-button')).click())
      //Wait for page to load and get Q2 url
      .then(() => browser.driver.wait(waitForPage, 5000))
      .then(() => browser.driver.getCurrentUrl())
      .then((q2Url) => {
        expect(q2Url.slice(-1)).toEqual('2');
      });
  }, 60000);
});

function waitForPage() {
  return browser.driver.executeScript('return document.readyState')
    .then((readyState) => readyState === 'complete');
}