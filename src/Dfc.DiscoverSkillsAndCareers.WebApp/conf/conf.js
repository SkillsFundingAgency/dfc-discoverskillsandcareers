const BrowserStackUser = process.env.BROWSER_STACK_USER || require('../Config/config').Browser_Stack_User;
const BrowserStackKey = process.env.SECRET_BROWSER_STACK_KEY || require('../Config/config').Browser_Stack_Key;
const buildNum = process.env.BUILD_BUILDNUMBER? process.env.BUILD_BUILDNUMBER : `local-${Date.now()}`;


const config = {
    commonCapabilities: {
        'browserstack.user': BrowserStackUser,
        'browserstack.key': BrowserStackKey,
        build: buildNum,
        project: 'Understand MySelf - National Careers Service',
        'browserstack.debug': 'true'
    },
  
    multiCapabilities: [
        {
            'name': 'Edge E2E Tests',
            'os': 'Windows',
            'os_version': '10',
            'browserName': 'Edge',
            'browser_version': '18.0',
            'resolution': '1024x768'
        },
        {
            'name': 'Chrome Win10 E2E Tests',
            'os': 'Windows',
            'os_version': '10',
            'browserName': 'Chrome',
            'browser_version': '72.0',
            'resolution': '1024x768'
        },
        {
            'name': 'Firefox Win10 E2E Tests',
            'os': 'Windows',
            'os_version': '10',
            'browserName': 'Firefox',
            'browser_version': '65.0',
            'resolution': '1024x768'
        },
        {
            'name': 'Chrome MacOS E2E Tests',
            'os': 'OS X',
            'os_version': 'Mojave',
            'browserName': 'Chrome',
            'browser_version': '71.0',
            'resolution': '1024x768'
        },
        {
            'name': 'Firefox MAcOS E2E Tests',
            'os': 'OS X',
            'os_version': 'Mojave',
            'browserName': 'Firefox',
            'browser_version': '64.0',
            'resolution': '1024x768'
        },
        {
            'name': 'Safari iOS E2E Tests',
            'browserName' : 'iPhone',
            'device': 'iPhone XS',
            'realMobile': 'true',
            'os_version': '12.1'
        },
        {
            'name': 'Chrome Android E2E Tests',
            'browserName' : 'android',
            'device': 'Samsung Galaxy S9',
            'realMobile': 'true',
            'os_version': '8.0'
        }
    ]
};
  
exports.capabilities = [];
// Code to support common capabilities
config.multiCapabilities.forEach(function(caps) {
  var temp_caps = JSON.parse(JSON.stringify(config.commonCapabilities));
  for(var i in caps) temp_caps[i] = caps[i];
  exports.capabilities.push(temp_caps);
});