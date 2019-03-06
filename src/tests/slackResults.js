const axios = require('axios');
const testResults = require('./log/results');
const slackWebHook = process.env.NCS_INT_SLACK || require('./conf/config').slackWebHook.DEV_SLACK;

const buildNum = process.env.BUILD_BUILDNUMBER? process.env.BUILD_BUILDNUMBER : '0';
const buildDefName = process.env.BUILD_DEFINITIONNAME? process.env.BUILD_DEFINITIONNAME : 'local';

const pa11yTestPassed = Object.keys(testResults.release.pa11y).length? Object.keys(testResults.release.pa11y).every((page) => testResults.release.pa11y[page].passed) : false;
const lighthouseTestPassed = Object.keys(testResults.release.lighthouse).length? Object.keys(testResults.release.lighthouse).every((page) => testResults.release.lighthouse[page].score >= 0.9) : false;
// const browserStackTestFailed = testResults.release.browserStack.length > 0;

axios.post(slackWebHook, {
     text: `Front-end Test Results for build number ${buildNum} from ${buildDefName}:`,
     attachments: [
         {
             title: "Pa11y",
             text: pa11yTestPassed? 'Passed' : 'Failed',
             color: pa11yTestPassed? 'good' : 'warning'
         },
         {
             title: "Lighthouse",
             text: lighthouseTestPassed? 'Passed' : 'Failed',
             color: lighthouseTestPassed? 'good' : 'warning'
         },
         {
             title: "Browser Stack",
             text: 'Disabled',
             color: 'warning'
         }
     ]
 }).then(() => {
     if (!pa11yTestPassed || !lighthouseTestPassed) process.exit(1);
 });