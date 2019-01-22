describe('National Careers Service', function() {
  it('Find title of question page', function() {
    browser.driver.get('https://ncstestdiscoverskills.z6.web.core.windows.net/').then(function() {
      browser.driver.findElement(By.linkText('Start assessment')).click().then(() => {
        browser.driver.findElement(By.linkText('Start now')).click().then(() => {
          expect(browser.driver.getTitle()).toEqual('National Careers Service');
        });
      });
    });
  });
});