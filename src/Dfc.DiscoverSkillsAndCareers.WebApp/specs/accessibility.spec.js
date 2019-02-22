const pa11y = require('pa11y');
const {expect} = require('chai');
const fs = require('fs');
const resultsJSON = require('../log/results')
const results = {
    home: {passed: false, issues: []},
    start: {passed: false, issues: []},
    statement: {passed: false, issues: []},
    saveProgress: {passed: false, issues: []},
    finish: {passed: false, issues: []},
    results: {passed: false, issues: []}
};
const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};

describe('Accessibility test Understand Me html pages', function () {
    this.timeout(60000);

    after(function() {
        resultsJSON.release.pa11y = results;
        fs.writeFileSync('./log/results.json', JSON.stringify(resultsJSON));
    });

    it('Home page', () => {
        // TODO: change url to dev env once known
        return pa11y(`https://dfc-my-skillscareers-mvc.azurewebsites.net`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            if (issues.length) results.home.issues = issues;
            else results.home.passed = true; 
            expect(issues).to.eql([]);
        });
    });

    it('Start page', () => {
        // TODO: change url to dev env once known
        return pa11y(`https://dfc-my-skillscareers-mvc.azurewebsites.net/start`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            if (issues.length) results.start.issues = issues;
            else results.start.passed = true; 
            expect(issues).to.eql([]);
        });
    });

    it('Statement page', () => {
        // TODO: change url to dev env once known
        return pa11y(`https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            if (issues.length) results.statement.issues = issues;
            else results.statement.passed = true; 
            expect(issues).to.eql([]);
        });
    });

    it('Save Progress page', () => {
        // TODO: change url to dev env once known
        return pa11y('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1', {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-link--no-visited-state',
                'wait for path to be /save-my-progress'
            ]
        }).then(({issues}) => {
            if (issues.length) results.saveProgress.issues = issues;
            else results.saveProgress.passed = true; 
            expect(issues).to.eql([]);
        });
    });

    it('Finish page', () => {
        // TODO: change url to dev env once known
        return pa11y('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1', {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/2',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/3',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/4',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /finish'
            ]
        }).then(({issues}) => {
            if (issues.length) results.finish.issues = issues;
            else results.finish.passed = true; 
            expect(issues).to.eql([]);
        })
    });

    it('Results page', () => {
        // TODO: change url to dev env once known
        return pa11y('https://dfc-my-skillscareers-mvc.azurewebsites.net/q/1', {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/2',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/3',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/4',
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /finish',
                'click element .govuk-button',
                'wait for path to be /results/',
                'screen capture results.png'
            ]
        }).then(({issues}) => {
            if (issues.length) results.results.issues = issues;
            else results.results.passed = true; 
            expect(issues).to.eql([]);
        })
    });
});
