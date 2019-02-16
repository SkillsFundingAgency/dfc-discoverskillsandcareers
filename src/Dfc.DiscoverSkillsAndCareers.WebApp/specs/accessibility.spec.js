const pa11y = require('pa11y');
const {expect} = require('chai');
const fs = require('fs');
const resultsJSON = require('../log/results')
const failures = [];
const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};

describe('Accessibility test Understand Me html pages', function () {
    this.timeout(30000);

    it('Home page', () => {
        return pa11y(`https://localhost:5001`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            failures.push(addPageName(issues, 'Home'));
            expect(issues).to.eql([]);
        });
    });

    it('Start page', () => {
        return pa11y(`https://localhost:5001/start`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            failures.push(addPageName(issues, 'Start'));
            expect(issues).to.eql([]);
        });
    });

    it('Statement page', () => {
        return pa11y(`https://localhost:5001/q/1`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        }).then(({issues}) => {
            failures.push(addPageName(issues, 'Statement'));
            expect(issues).to.eql([]);
        });
    });

    it('Save Progress page', () => {
        return pa11y('https://localhost:5001/q/1', {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-link--no-visited-state',
                'wait for path to be /save-my-progress'
            ]
        }).then(({issues}) => {
            failures.push(addPageName(issues, 'Save Progress'));
            expect(issues).to.eql([]);
        });
    });

    it('Finish page', () => {
        return pa11y('https://localhost:5001/q/1', {
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
            failures.push(addPageName(issues, 'Finish'));
            expect(issues).to.eql([]);
        })
    });

    it('Results page', () => {
        return pa11y('https://localhost:5001/q/1', {
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
            failures.push(addPageName(issues, 'Results'));
            resultsJSON.release.pa11y = failures;
            fs.writeFileSync('./log/results.json', JSON.stringify(resultsJSON));
            expect(issues).to.eql([]);
        })
    });
});

function addPageName (issues, pageName) {
    if (issues.length) {
        const issuesWithPageName = issues.map(issue => {
            issue.page = pageName;
            return issue;
        });
        return issuesWithPageName;
    } else return {
        type: 'Pass',
        page: pageName
    };
}
