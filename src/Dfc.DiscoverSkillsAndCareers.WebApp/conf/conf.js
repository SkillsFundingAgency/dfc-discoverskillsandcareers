const {BrowserStackKey, BrowserStackUser} = require('./config');

exports.config = {
    'specs': [ '../specs/browser.spec.js' ],
    'browserstackUser': process.env.BrowserStackUser || BrowserStackUser,
    'browserstackKey': process.env.BrowserStackKey|| BrowserStackKey,
    
    'commonCapabilities': {
      'build': 'private_beta',
      'name': 'ncs_parallel_test',
      'browserstack.debug': 'true'
    },
  
    'multiCapabilities': [
    {
        'os': 'Windows',
        'os_version': '10',
        'browserName': 'Edge',
        'browser_version': '18.0',
        'resolution': '1024x768'
    },
    {
        'os': 'Windows',
        'os_version': '10',
        'browserName': 'Chrome',
        'browser_version': '71.0',
        'resolution': '1024x768'
    },
    {
        'os': 'Windows',
        'os_version': '10',
        'browserName': 'Firefox',
        'browser_version': '64.0',
        'resolution': '1024x768'
    },
    {
        'os': 'OS X',
        'os_version': 'Mojave',
        'browserName': 'Safari',
        'browser_version': '12.0',
        'resolution': '1024x768'
    },
    {
        'os': 'OS X',
        'os_version': 'Mojave',
        'browserName': 'Chrome',
        'browser_version': '71.0',
        'resolution': '1024x768'
    },
    {
        'os': 'OS X',
        'os_version': 'Mojave',
        'browserName': 'Firefox',
        'browser_version': '64.0',
        'resolution': '1024x768'
    },
    {
        'browserName' : 'iPhone',
        'device': 'iPhone XS',
        'realMobile': 'true',
        'os_version': '12.1'
    },
    {
        'browserName' : 'android',
        'device': 'Samsung Galaxy S9',
        'realMobile': 'true',
        'os_version': '8.0'
    }]
  };
  
  // Code to support common capabilities
  exports.config.multiCapabilities.forEach(function(caps){
    for(var i in exports.config.commonCapabilities) caps[i] = caps[i] || exports.config.commonCapabilities[i];
  });