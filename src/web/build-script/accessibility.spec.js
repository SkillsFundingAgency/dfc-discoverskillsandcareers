const pa11y = require('pa11y');
const {expect} = require('chai');
const fs = require('fs');

describe('Accessibility testing for web pages', function () {
    this.timeout(5000);
    // Create index of html pages
    const htmlPages = fs.readdirSync('./src/templates');

    // Pa11y test each html page
    htmlPages.forEach(page => {
        it(page, () => {
            return pa11y(`http://localhost:3000/${page}`)
                .then(({issues}) => {
                    expect(issues).to.eql([]);
                });
        });
    })
});