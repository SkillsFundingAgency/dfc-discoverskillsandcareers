const pa11y = require('pa11y');
const {expect} = require('chai');

describe('Accessibility testing for web pages', function () {
    this.timeout(5000);
    it('Question page', () => {
        return pa11y('https://accessibility.18f.gov/checklist/')
            .then(({issues}) => {
                expect(issues.length).to.equal(0);
            });
    });

    it('Results page', () => {
        return pa11y('https://accessibility.18f.gov/color/')
            .then(({issues}) => {
                expect(issues.length).to.equal(0);
            });
    });

    it('Landing page', () => {
        return pa11y('https://accessibility.18f.gov/color/')
            .then(({issues}) => {
                expect(issues.length).to.equal(0);
            });
    });

    it('Preamble page', () => {
        return pa11y('https://accessibility.18f.gov/color/')
            .then(({issues}) => {
                expect(issues.length).to.equal(0);
            });
    });
});