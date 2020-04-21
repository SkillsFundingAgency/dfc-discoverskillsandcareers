var helpers = (function () {
  return {
    isPage: function (className) {
      return document.getElementsByClassName(className).length
      },
    getExplorePagesLink : function (linkPath) {
        var domain = window.location.hostname;
        if (domain.includes('dev') || domain.includes('local') ) {
            return 'https://dev-beta.nationalcareersservice.org.uk' + linkPath 
        } else if (domain.includes('sit'))  {
            return 'https://sit-beta.nationalcareersservice.org.uk' + linkPath
        } else if (domain.includes('staging')) {
            return 'https://staging.nationalcareers.service.gov.uk' + linkPath
        }     
        return 'https://nationalcareers.service.gov.uk' + linkPath
      }
  }
})()

module.exports = helpers
