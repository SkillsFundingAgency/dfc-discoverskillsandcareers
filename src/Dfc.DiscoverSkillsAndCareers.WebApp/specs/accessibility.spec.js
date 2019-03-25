const pa11y = require('pa11y');
const {expect} = require('chai');
const customHostName = process.env.CustomHostName? process.env.CustomHostName : require('../Config/config').CustomHostName;

const answerDict = {
    'Strongly agree': 'selected_answer-1',
    'Agree': 'selected_answer-2',
    'Disagree': 'selected_answer-3',
    'Strongly disagree': 'selected_answer-4',
    "This doesn't apply to me": 'selected_answer-5'
};

const appUrl = `https://${customHostName}`;

describe('Pa11y accessibility testing for Understand Myself - National Careers Service', function () {
    this.timeout(120000);

    it('Home page', async () => {
        const {issues} = await pa11y(appUrl, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        });

        expect(issues).to.eql([]);
    });

    it('Statement page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            timeout: 60000,
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-button--start',
                'wait for path to be /q/1'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Save Progress page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            timeout: 90000,
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-button--start',
                'wait for element .govuk-link--no-visited-state to be added',
                'click element .govuk-link--no-visited-state',
                'wait for path to be /save-my-progress'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Finish page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            timeout: 180000,
            actions: [
                'click element .govuk-button--start',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/2',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/3',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/4',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/5',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/6',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/7',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/8',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/9',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/10',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/11',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/12',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/13',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/14',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/15',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/16',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/17',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/18',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/19',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/20',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/21',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/22',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/23',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/24',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/25',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/26',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/27',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/28',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/29',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/30',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/31',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/32',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/33',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/34',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/35',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/36',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/37',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/38',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/39',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/40',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /finish'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Results page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            timeout: 180000,
            actions: [
                'click element .govuk-button--start',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/2',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/3',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/4',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/5',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/6',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/7',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/8',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/9',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/10',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/11',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/12',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/13',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/14',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/15',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/16',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/17',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/18',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/19',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/20',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/21',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/22',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/23',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/24',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/25',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/26',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/27',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/28',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/29',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/30',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/31',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/32',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/33',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/34',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/35',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/36',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/37',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/38',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/39',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /q/40',
                `wait for element #${answerDict['Agree']} to be added`,
                `check field #${answerDict['Agree']}`,
                'click element .govuk-button',
                'wait for path to be /finish',
                'wait for element .govuk-button to be added',
                'click element .govuk-button',
                'wait for path to be /results'
            ]
        });

        expect(issues).to.eql([]);
    });
});
