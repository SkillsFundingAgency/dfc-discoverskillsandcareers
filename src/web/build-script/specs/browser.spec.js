describe('Understand Myself', function() {
  it('Find title of question page', function() {
    browser.driver.get('http://localhost:3000').then(function() {
      browser.driver.findElement(By.linkText('Start assessment')).click().then(() => {
        browser.driver.findElement(By.linkText('Start now')).click().then(() => {
          expect(browser.driver.getTitle()).toEqual('Understand Myself');
        });
      });
    });
  });
});