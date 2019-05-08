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

const assessmentRunThroughIns = [
    'click element .govuk-button--start',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/02',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/03',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/04',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/05',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/06',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/07',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/08',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/09',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/10',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/11',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/12',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/13',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/14',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/15',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/16',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/17',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/18',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/19',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/20',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/21',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/22',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/23',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/24',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/25',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/26',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/27',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/28',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/29',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/30',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/31',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/32',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/33',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/34',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/35',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/36',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/37',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/38',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/39',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button',
    'wait for path to be /q/short/40',
    `wait for element #${answerDict['Agree']} to be added`,
    `check field #${answerDict['Agree']}`,
    'click element .govuk-button'
];

describe('Pa11y accessibility testing for Understand Myself - National Careers Service', function () {
    this.timeout(240000);

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
            timeout: 900000,
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-button--start',
                'wait for path to be /q/short/01'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Save Progress page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            timeout: 120000,
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

    it('Save Progress Reference page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            timeout: 120000,
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            actions: [
                'click element .govuk-button--start',
                'wait for element .govuk-link--no-visited-state to be added',
                'click element .govuk-link--no-visited-state',
                'wait for path to be /save-my-progress',
                'wait for element #SelectedOption-2 to be added',
                'check field #SelectedOption-2',
                'click element .govuk-button',
                'wait for path to be /save-my-progress/reference'
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
                ...assessmentRunThroughIns,
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
                ...assessmentRunThroughIns,
                'wait for path to be /finish',
                'wait for element .govuk-button to be added',
                'click element .govuk-button',
                'wait for path to be /results'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Filtering statements page', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            timeout: 180000,
            actions: [
                ...assessmentRunThroughIns,
                'wait for path to be /finish',
                'wait for element .govuk-button to be added',
                'click element .govuk-button',
                'wait for path to be /results',
                'wait for element .app-button to be added',
                'click element .app-button',
                'wait for path to be /q/social-care/01'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('Result page with infographics', async () => {
        const {issues} = await pa11y(`${appUrl}`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"],
            timeout: 180000,
            actions: [
                ...assessmentRunThroughIns,
                'wait for path to be /finish',
                'wait for element .govuk-button to be added',
                'click element .govuk-button',
                'wait for path to be /results',
                'wait for element .app-button to be added',
                'click element .app-button',
                'wait for path to be /q/social-care/01',
                'wait for element #selected_answer-1 to be added',
                'check field #selected_answer-1',
                'click element .govuk-button',
                'wait for path to be /q/social-care/02',
                'wait for element #selected_answer-1 to be added',
                'check field #selected_answer-1',
                'click element .govuk-button',
                'wait for path to not be /q/social-care/02',
                'wait for element .govuk-button to be added',
                'click element .govuk-button',
                'wait for element .app-masthead__title to be added'
            ]
        });

        expect(issues).to.eql([]);
    });

    it('404 page', async () => {
        const {issues} = await pa11y(`${appUrl}/dummy`, {
            standard: "WCAG2AA",
            // Rule ignored due to problem in GOV template
            ignore: ["WCAG2AA.Principle1.Guideline1_3.1_3_1.F92,ARIA4"]
        });

        expect(issues).to.eql([]);
    });
});
