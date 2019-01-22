const browserstack = require('browserstack-local');

exports.config = {
    'specs': [ '../specs/*.spec.js' ],
    'browserstackUser': 'gohariqbal1',
    'browserstackKey': 'YMUAYzQaieSxxseByonh',
    
    'commonCapabilities': {
      'build': 'beta',
      'name': 'ncs_parallel_test',
      'browserstack.local': true,
      'browserstack.debug': 'true'
    },
  
    'multiCapabilities': [{
        'os': 'Windows',
        'os_version': '10',
        'browserName': 'IE',
        'browser_version': '11.0',
        'resolution': '1024x768'
    },
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
        'browser_version': '72.0 beta',
        'resolution': '1024x768'
    },
    {
        'os': 'Windows',
        'os_version': '10',
        'browserName': 'Firefox',
        'browser_version': '65.0 beta',
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
        'browser_version': '72.0 beta',
        'resolution': '1024x768'
    },
    {
        'os': 'OS X',
        'os_version': 'Mojave',
        'browserName': 'Firefox',
        'browser_version': '65.0 beta',
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
    }],

    // Code to start browserstack local before start of test
    beforeLaunch: function(){
        console.log("Connecting local");
        return new Promise(function(resolve, reject){
            exports.bs_local = new browserstack.Local();
            exports.bs_local.start({'key': exports.config['browserstackKey'] }, function(error) {
                if (error) return reject(error);
                console.log('Connected. Now testing...');

                resolve();
            });
        });
    },

    // Code to stop browserstack local after end of test
    afterLaunch: function(){
        return new Promise(function(resolve, reject){
            exports.bs_local.stop(resolve);
        });
    }
  };
  
  // Code to support common capabilities
  exports.config.multiCapabilities.forEach(function(caps){
    for(var i in exports.config.commonCapabilities) caps[i] = caps[i] || exports.config.commonCapabilities[i];
  });