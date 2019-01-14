describe('Understand Myself', function() {
  it('Can find page title', function() {
    browser.driver.get('http://localhost:3000').then(function() {
      browser.driver.findElement(By.linkText('Start assessmemt')).click().then(() => {
        expect(browser.driver.getTitle()).toEqual('Understand Myself');
      });
    });
  });
});