const {expect} = require('chai');
const fs = require('fs');
const lighthouse = require('lighthouse');
const chrome = require('chrome-launcher');

// Create index of html pages
const htmlPages = fs.readdirSync('./src/templates');

// launches chrome and performs a lighthouse test
function launchChromeAndRunLighthouse(url, opts, config = null) {
    return chrome.launch({chromeFlags: opts.chromeFlags}).then(instance => {
      opts.port = instance.port;
      return lighthouse(url, opts, config).then(results => {
        return instance.kill().then(() => results.lhr)
      });
    });
}

describe('Lighthouse performance testing for web pages', function() {
    this.timeout(20000);
    const opts = {
        chromeFlags: ['--headless'],
        onlyCategories: ['performance']
    };
    const performanceThreshold = 0.90;

    // Lighthouse performance test each html page
    htmlPages.forEach(page => {
        it(page, () => {
            return launchChromeAndRunLighthouse(`http://localhost:3000/${page}`, opts).then(({categories}) => {
                expect(categories.performance.score).to.be.above(performanceThreshold);
            });
        });
    });
});