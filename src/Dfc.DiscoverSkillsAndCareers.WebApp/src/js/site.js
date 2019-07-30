const helpers = require('./modules/helpers.js')
const analytics = require('./modules/analytics.js')
const results = require('./modules/results.js')
const breadcrumbs = require('./modules/breadcrumbs.js')
const GOVUKFrontend = require('govuk-frontend')

GOVUKFrontend.initAll()
//analytics.init()
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
