const helpers = require('./modules/helpers.js')
const results = require('./modules/results.js')
const analytics = require('./modules/analytics.js')
const breadcrumbs = require('./modules/breadcrumbs.js')
const cookieprefrences = require('./modules/cookieprefrences.js')
const GOVUKFrontend = require('govuk-frontend')

GOVUKFrontend.initAll()
analytics.init()
breadcrumbs.init()

if (helpers.isPage('app-page--results')) {
  results.short()
}

if (helpers.isPage('app-page--results-long')) {
  if (document.body.clientWidth >= 768) {
    results.cardHeight()
  }
  results.long()
}

if (helpers.isPage('gem-c-cookie-banner')) {
  if (typeof cookieprefrences.setGATracking === 'undefined') document.body.className = document.body.className.replace('js-enabled', '')

  if (cookieprefrences.isCookiePrefrenceSet()) {
    document.getElementById('global-cookie-banner').style.display = 'none'
  } else {
    cookieprefrences.setDefault()

    document.getElementById('accept-all-cookies').addEventListener('click', function () {
      cookieprefrences.approveAll()
      document.getElementById('cookie-message').style.display = 'none'
      document.getElementById('confirmatiom-message').style.display = 'block'
    })

    document.getElementById('hide-cookies-message').addEventListener('click', function () {
      document.getElementById('global-cookie-banner').style.display = 'none'
    })

    var cookiePreference = document.getElementById('set-cookie-preference')
    cookiePreference.setAttribute('href', helpers.getExplorePagesLink(cookiePreference.getAttribute('href')))

    var cookieSettings = document.getElementById('cookie-settings-help')
    cookieSettings.setAttribute('href', helpers.getExplorePagesLink(cookieSettings.getAttribute('href')))
  }
}
